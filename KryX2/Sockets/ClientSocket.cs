using System;

using System.Net;
using System.Net.Sockets;
using KryX2.Client;

namespace KryX2.Sockets
{

    internal class ClientSocket : ClientData
    {

        internal Socket _socket;
        private Timing _timing;

        internal ClientSocket()
        {
            _timing = new Timing();
            _timing.ReferenceClientSocket(this);
        }

        internal void ResetTiming(bool RequireLoggedOff = false)
        {
            //if must be logged off to reset
            if (RequireLoggedOff)
            {
                if (!this.LoggedOn)
                {
                    _timing.StopTimer();
                    _timing.StartTimer();
                }
            }
            //reset regardless
            else
            {
                _timing.StopTimer();
                _timing.StartTimer();
            }
        }

        internal void StopTiming()
        {
            _timing.StopTimer();
        }

        internal void ConnectClient()
        {
            try
            {
                this.ProxyAuthorized = false;
                ResetTiming();
                _timing.StartTimer();
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint hostEndPoint = new IPEndPoint(this.ProxyAddress, this.ProxyPort);
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.RemoteEndPoint = hostEndPoint;
                eventArgs.UserToken = _socket;
                eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ClientConnected);
                _socket.ConnectAsync(eventArgs);
            }
            catch
            {
                SocketActions.ProcessError(this, "ConnectClient: catch");
            }
        }

        private void ClientConnected(object sender, SocketAsyncEventArgs e)
        {
            //free a semaphore
            SocketActions.ReleaseCurrentlyConnecting();
            //resets timeout
            ResetTiming();
            //determine if connection has been made
            bool connected = (e.SocketError == SocketError.Success);
            //define socketdata
            if (connected)
            {
                //sends header based on if using proxy or not and server type
                SocketActions.SendProxyHeader(this);
                //begins async receive
                StartReceive();
            }
            else
            {
                //failed to connect successfully
                SocketActions.ProcessError(this, "ClientConnected: failed to connect");
            }
        }

        //inserts information needed for async receive.
        //I could pass the asynceventargs instead of sd but for now
        //im leaving it at sd for clarification
        internal void StartReceive()
        {
            //if connection is closed
            if (SocketActions.IsConnectionClosed(_socket))
            {
                SocketActions.ProcessError(this, "StartReceive: connection closed");
                return;
            }

            //determine if there is data to be read.
            byte[] data = RetrieveReceived();
            if (data != null)
            {
                //let handlereceived decide what to do with the data
                SocketActions.HandleReceived(this, data);
                //exit method since we dont need to set a completion event as we already
                //know there is data
                return;
            }

            try
            {
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                byte[] buffer = new byte[512]; //255 //r/ 
                eventArgs.SetBuffer(buffer, 0, 512);
                eventArgs.UserToken = _socket;
                eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(DataReceived);
                _socket.ReceiveAsync(eventArgs);
            }
            catch
            {
                SocketActions.ProcessError(this, "StartReceive: catch");
            }
        }


        private byte[] RetrieveReceived(SocketAsyncEventArgs e)
        {

            try
            {
                int bytesRead = e.BytesTransferred;
                //if 0 socket has been closed
                if (bytesRead == 0)
                    return null;
                byte[] result = new byte[bytesRead];
                Buffer.BlockCopy(e.Buffer, 0, result, 0, bytesRead);

                return result;
            }
            catch
            {
                SocketActions.ProcessError(this, "RetrieveReceived(e): catch");
                return null;
            }
        }
        private byte[] RetrieveReceived()
        {
            try
            {
                int bytesRead = _socket.Available;
                if (bytesRead == 0)
                    return null;

                byte[] result = new byte[bytesRead];
                _socket.Receive(result);
                return result;
            }
            catch
            {
                SocketActions.ProcessError(this, "RetrieveReceived: catch");
                return null;
            }

        }

        internal void DataReceived(object sender, SocketAsyncEventArgs e)
        {
            byte[] data = RetrieveReceived(e);
            if (data != null)
            {
                SocketActions.HandleReceived(this, data);
            }
            //null array or 0 bytes means graceful socket close
            else
            {
                //SocketActions.ProcessError(this, "DataReceived: null data");
            }
        }


    }

}