
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using KryX2.FileManagement;
using KryX2.PacketManagement;
using KryX2.UI;
using System.Drawing;
using System.Timers;
using System.Threading;
using KryX2.Settings;

namespace KryX2.Sockets
{

    internal static class SocketActions
    {
        private static Semaphore _currentlyConnecting = new Semaphore(50, 50);
        private static readonly List<ClientSocket> _clientSockets = new List<ClientSocket>();

        internal static void SpawnAndConnectClients(int count)
        {
            Task.Factory.StartNew(() =>
            {
                //reset clientsocket list
                _clientSockets.Clear();

                int i = 0;
                do
                {
                    if (!CreateNewClient())
                    {
                        Chat.Add(System.Drawing.Color.Yellow, "Failed to create all clients. " + i + " clients made.");
                        break;
                    }
                    //if couldnt create a new socket leave loop
                    i++;
                } while (i < count);

                //StartReconnectTimer();
                ConnectAllClients();

            });
        }

        internal static void DisconnectAllClients()
        {
           // EndReconnectTimer();
            for (int i = 0; i < _clientSockets.Count; i++)
            {
                DisconnectClient(_clientSockets[i]);
            }
            try
            {
                _currentlyConnecting = new Semaphore(50, 50);
            }
            catch { }
        }
        internal static void ConnectAllClients()
        {

            for (int i = 0; i < _clientSockets.Count; i++)
            {
                RelayConnectClient(_clientSockets[i]);
            }
        }

        internal static void RelayConnectClient(ClientSocket cs)
        {
            if (GeneratedSettings.BotActive == false)
                return;

            _currentlyConnecting.WaitOne();
            cs.ConnectClient();
        }
        internal static void ReleaseCurrentlyConnecting()
        {
            try
            {
                _currentlyConnecting.Release(1);
            }
            catch { }
        }

        private static bool CreateNewClient()
        {
            ClientSocket cs = new ClientSocket();

            if (!Proxies.CycleProxy(cs))
            {
                Chat.Add(Color.Yellow, "Proxy is null" + Environment.NewLine);
                return false;
            } //exit method due to no more proxies being available


            //set cdkey
            if (!CDKeys.CycleCDKey(cs))
            {
                Chat.Add(Color.Yellow, "CDKey is null" + Environment.NewLine);
                return false;
            }

            cs.Active = true;
            _clientSockets.Add(cs);
            return true;
        }

        internal static void DisconnectClient(ClientSocket cs)
        {
            cs.StopTiming();
            ConnectedBar.RemoveOne(cs);
            cs.Channel = string.Empty;
            cs.LoggedOn = false;
            cs.ServerToken = 0;
            cs.ClientToken = 0;
            cs.PacketBuffer.ResetBuffer();
            try
            {
                cs._socket.Close();
            }
            catch { }
        }

        internal static void HandleReceived(ClientSocket cs, byte[] data)
        {
            if (!cs.ProxyAuthorized)
            {
                //if using proxy authorize it
                AuthorizeProxy(cs, data);
            }
            else
            {
                ProcessReceivedData(cs, data);
            }
            cs.StartReceive();
        }

        //determines if a socket has been closed.
        internal static bool IsConnectionClosed(Socket socket)
        {
            try
            {
                bool readAvailable = socket.Poll(0, SelectMode.SelectRead);
                if (!readAvailable)
                    return false;

                if (socket.Available == 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }


        internal static void SendData(ClientSocket cs, byte[] data)
        {
            //if _connected is true, as far as we know the connection is still
            //established with the host
            if (cs._socket.Connected)
            {
                try
                {
                    cs._socket.Send(data);
                }
                catch (Exception)
                {
                    //send failed, socket must of disconnected without notice
                    ProcessError(cs, "SendData: Catch");
                }
            }
            else
            {
                //socket became disconnected, we dont really seem to know when
                ProcessError(cs, "SendData: not connected");
            }
        }

        // Close socket in case of failure and throws
        // a SockeException according to the SocketError.
        internal static void ProcessError(ClientSocket cs, string reason)
        {
            //if (cs.LoggedOn)
              //  Debug.Print("PE " + reason);
            ReconnectClient(cs, false, true);
        }


        internal static void AuthorizeProxy(ClientSocket cs, byte[] byteData)
        {
            switch (cs.ProxyFormat)
            {
                case ProxyType.Http:
                    string dataString = Encoding.ASCII.GetString(byteData);
                    if (dataString.IndexOf("200 OK", StringComparison.Ordinal) != -1)
                    {
                        cs.ProxyAuthorized = true;
                    }
                    break;
                case ProxyType.Socks4:
                    if (byteData.Length > 1)
                    {
                        if ((byteData[1] == 90) &&
                            (byteData[0] == 0))
                        {
                            cs.ProxyAuthorized = true;
                        }
                    }
                    break;
                case ProxyType.Socks5:
                    //not handling socks 5 atm
                    break;
            }
            //perform actions based on whichever proxy type
            if (cs.ProxyAuthorized)
            {
                cs.PacketBuffer.ResetBuffer();
                PacketParser.Send0x50(cs);
            } //if authorized send x50 
            else
            {
                //proxy denied use, try another one
                //ReconnectClient(cs, false, true);
            }
        } //end authorizeproxy


        internal static void SendProxyHeader(ClientSocket cs)
        {

            //r/ if proxy format isn't specified assume socks4. normally we'd let user decide default if not specified or determined.
            if (cs.ProxyFormat == ProxyType.Default)
            {
                cs.ProxyFormat = ProxyType.Http;
            }

            switch (cs.ProxyFormat)
            {
                case ProxyType.Http:
                    SocketActions.SendData(cs, KryX2.Settings.GeneratedSettings.HttpHeader);
                    break;
                case ProxyType.Socks4:
                    SocketActions.SendData(cs, KryX2.Settings.GeneratedSettings.Socks4Header);
                    break;
                case ProxyType.Socks5:
                    //socks5 is currently unused
                    break;
            }
        }

        internal static void ProcessReceivedData(ClientSocket cs, byte[] data)
        {

            cs.PacketBuffer.Append(data);

            while (cs.PacketBuffer.ReturnDataLength() > 3)
            {
                byte[] retrievedPacket = cs.PacketBuffer.ReturnFullPacket();
                if (retrievedPacket != null)
                {
                    PacketParser.ParsePackets(cs, retrievedPacket);
                }
                else
                {
                    break;
                }
            }
        }

        internal static void ReconnectClient(ClientSocket cs, bool newCdKey = false, bool newProxy = false)
        {
            if (GeneratedSettings.BotActive == false)
                return;

            //always use new proxy, probably safer/more reliable for all clients to load
            newProxy = true;

            DisconnectClient(cs);

            //if key was marked as bad get a new one no matter what
            if (cs.CDKeyBlacklisted)
            {
                newCdKey = true;
                cs.CDKeyBlacklisted = false;
            }

            if (newCdKey)
            {
                if (!CDKeys.CycleCDKey(cs))
                {
                    cs.Active = false;
                    Debug.Print("CDKeys are out");
                    return;
                }
            }

            if (newProxy)
            {
                if (!Proxies.CycleProxy(cs))
                {
                    cs.Active = false;
                    Debug.Print("Proxies are out");
                    Chat.Add(Color.Red, "Proxies are out" + Environment.NewLine);
                    return;
                }
            }

            RelayConnectClient(cs);
            //cs.ConnectClient(); /r// semaphore
        }



    }
}

