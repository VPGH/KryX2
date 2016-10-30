using System;
using System.Timers;
using KryX2.Sockets;
using KryX2.Settings;
using KryX2.PacketManagement;
using System.Diagnostics;

namespace KryX2.Client
{
    internal class Timing
    {

        private Timer _timer = new System.Timers.Timer();
        private ClientSocket _clientSocket;

        private void InitializeSettings()
        {
            _timer.Interval = 15000;
            _timer.Elapsed += new ElapsedEventHandler(TimerFired);
            StopTimer();
        }
        internal void ReferenceClientSocket(ClientSocket cs)
        {
            _clientSocket = cs;
            InitializeSettings();
        }

        internal void StartTimer()
        {
            _timer.Enabled = true;
            _timer.Start();
        }

        internal void StopTimer()
        {
            _timer.Enabled = false;
            _timer.Stop();
        }

        private void TimerFired(object sender, ElapsedEventArgs e)
        {
            //if currently logged on
            if (_clientSocket.LoggedOn)
            {
                //if not in target channel
                if (_clientSocket.Channel != UserSettings.TargetChannel)
                {
                    //send join
                    PacketParser.Send0x0C(_clientSocket);
                }
                //if in target channel
                else
                {
                    //Builder builder = new Builder();
                    //builder.InsertNTString(UserSettings.IdleMessage);
                    //builder.SendPacket(_clientSocket, 0x0e);
                }
            }
            //if not logged on yet
            else
            {
                SocketActions.ReconnectClient(_clientSocket, false, true);
            }
        }


    }
}
