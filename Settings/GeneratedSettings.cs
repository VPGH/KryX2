using System;
using System.Net;
using System.Drawing;
using KryX2.UI;
using KryX2.MBNCSUtil.CR;

namespace KryX2.Settings
{
    static class GeneratedSettings
    {
        internal static bool BotActive = false;

        internal static string AppDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        internal static byte[] HttpHeader { get; set; }
        internal static byte[] Socks4Header { get; set; }
        //internal static string Socks5Header { get; set; }

        //paths
        internal static string BadCDKeysText { get; set; }
        internal static string StarcraftExe { get; set; }
        internal static string StormDll { get; set; }
        internal static string BattleSnp { get; set; }
        internal static string StarBin { get; set; }
        internal static string LockdownPath { get; set; }
        internal static string[] HashFiles { get; set; }
        
        internal static void SetFilePaths()
        {
            BadCDKeysText = AppDirectory + @"\Text\BadCdKeys.txt";
            StarcraftExe = AppDirectory + @"\Star\Starcraft.exe";
            StormDll = AppDirectory + @"\Star\Storm.dll";
            BattleSnp = AppDirectory + @"\Star\Battle.snp";
            StarBin = AppDirectory + @"\Lockdown\Star.bin";
            LockdownPath = AppDirectory + @"\Lockdown\";
            SetHashFiles();
            FilesBuffered.LoadFiles();
        }
        internal static void SetHashFiles()
        {
            HashFiles = new string[]
                {
                    AppDirectory + @"\Star\Starcraft.exe",
                    AppDirectory + @"\Star\Storm.dll",
                    AppDirectory + @"\Star\Battle.snp"
                };
        }

        //builds headers for proxy types which are sent when connecting to a proxy
        #region Contruct Proxy Destination Headers
        internal static bool ConstructHeaders(string server)
        {
            try
            {
                IPAddress resultIP;
                if (IPAddress.TryParse(server, out resultIP))
                {
                }
                else
                {
                    IPAddress[] ips;
                    ips = Dns.GetHostAddresses(server);
                    Random randomizer = new Random();
                    int serverIndex = randomizer.Next(0, ips.Length - 1);
                    resultIP = ips[serverIndex];
                }

                if (resultIP.Equals(IPAddress.None) || resultIP.ToString().Length == 0 || resultIP.Equals(null))
                {
                    return false;
                }
                else
                {
                    Chat.Add(Color.GreenYellow, Environment.NewLine + "Using server address: " + resultIP.ToString() + Environment.NewLine);
                }

                int bnetPort = 6112;

                byte[] socks4Array = new byte[9];
                socks4Array[0] = 4;
                socks4Array[1] = 1;
                //socks 4 header
                Array.Copy(PortToBytes(bnetPort), 0, socks4Array, 2, 2);
                //copies port to array                
                byte[] IPArray;
                //IPArray = IPAddress.Parse(bnetServer).GetAddressBytes();
                IPArray = resultIP.GetAddressBytes();
                Array.Copy(IPArray, 0, socks4Array, 4, 4);
                //copy ip bytes to array
                socks4Array[8] = 0;
                //null trunc for username
                Socks4Header = new byte[socks4Array.Length]; //set to appropriate length
                Array.Copy(socks4Array, 0, Socks4Header, 0, 9);
                //build socks4 header

                string httpHeaderText = "CONNECT " + resultIP.ToString() + ":" + bnetPort + " HTTP/1.1" + Environment.NewLine +
                                        Environment.NewLine;
                HttpHeader = System.Text.Encoding.UTF8.GetBytes(httpHeaderText.ToCharArray());

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] PortToBytes(int port)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)(port / 256);
            ret[1] = (byte)(port % 256);
            return ret;
        }
        #endregion

    }
}
