using System;
using System.Windows.Forms;
using KryX2.Settings;
using KryX2.FileManagement;
using KryX2.UI;
using KryX2.Sockets;
using System.Drawing;

namespace KryX2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UserSettings.GenerateSettings();
            GeneratedSettings.SetFilePaths();
            GeneratedSettings.SetHashFiles();
            Chat.SetRichTextBox(this.chatBox);
            Proxies.ImportProxylist();
            CDKeys.ImportCdKeys();
            ConnectedBar.Reference(this.labelConnectedForecolor);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            //no keys, dont proceed
            
            int keyCount = CDKeys.ReturnListCount();
            int proxyCount = Proxies.ReturnListCount();

            if (keyCount == 0)
            {
                Chat.Add(Color.Yellow, "No cdkeys. Connection stopped." + Environment.NewLine);
                return;
            }
            if (proxyCount == 0)
            {
                Chat.Add(Color.Yellow, "No proxies. Connection stopped." + Environment.NewLine);
                return;
            }

            //determine max number of socks allowed
            int socks = UserSettings.MaxClients;
            int perProxy = UserSettings.ClientsPerProxy;
            int maxSockets = (perProxy * proxyCount);
            if (socks > maxSockets)
                socks = maxSockets;

            if (socks > keyCount)
                socks = keyCount;

            if (socks == 0)
            {
                Chat.Add(Color.Yellow, "Bot count cannot be zero. Connection stopped." + Environment.NewLine);
                return;
            }

            if (!GeneratedSettings.ConstructHeaders(UserSettings.ServerName))
            {
                Chat.Add(Color.Yellow, "Could not validate server. Connection stopped." + Environment.NewLine);
                return;
            }

            GeneratedSettings.BotActive = true;

            ConnectedBar.SetTotalSockets(socks);

            Chat.Add(Color.White, "Loading " + socks + " sockets." + Environment.NewLine);

            SocketActions.SpawnAndConnectClients(socks);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GeneratedSettings.BotActive = false;
            SocketActions.DisconnectAllClients();
        }
    }
}
