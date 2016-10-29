using System;
using System.Diagnostics;
using System.Drawing;
using BNSharp.BattleNet.Core;
using System.Text;
using CheckRevision = BNSharp.BattleNet.Core.CheckRevision;
using OldAuth = BNSharp.BattleNet.Core.OldAuth;
using KryX2.UI;
using KryX2.Sockets;
using KryX2.Settings;
using KryX2.FileManagement;

namespace KryX2.PacketManagement
{




    public static class PacketParser
    {

        private static string RandomName()
        {

            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder result = new StringBuilder();
            Random rnd = new Random();

            int nameLength = rnd.Next(9, 15);

            for (int i = 0; i < nameLength; i++)
            {
                result.Append(validCharacters[rnd.Next(validCharacters.Length)]);
            }

            return result.ToString();

        }

        internal static void Send0x0C(ClientSocket cs)
        {
            Builder builder = new Builder();
            builder.InsertInt32(2);
            if (cs.FirstJoin)
                builder.InsertNTString(RandomName());
            else
                builder.InsertNTString(UserSettings.TargetChannel);

            builder.SendPacket(cs, 0x0C);

        }

        internal static void ParsePackets(ClientSocket cs, byte[] data)
        {
            cs.ResetTiming(true);

            Receiver receiver = new Receiver(data);
            receiver.SkipLength(1); //0xff header
            byte packetId = receiver.GetByte();
            receiver.SkipLength(2); //data length, normally int16

            //Debug.Print("packet is " + packetId.ToString("X"));

            switch (packetId)
            {
                case 0x50:
                    Parse0x50(cs, receiver);
                    break;
                case 0x51:
                    Parse0x51(cs, receiver);
                    break;
                case 0x25:
                    Parse0x25(cs);
                    break;
                case 0x3A:
                    Parse0x3A(cs, receiver);
                    break;
                case 0x3D:
                    Parse0x3D(cs, receiver);
                    break;
                case 0x0F:
                    Parse0x0F(cs, receiver);
                    break;
                case 0x59:
                    //do nothing, email req packet
                    break;
                case 0x0a:
                    Parse0x0A(cs);
                    break;
                case 0x00:
                    Parse0x00(cs);
                    break;
                    //default:
                    //break;
            }

        }

        private static void Parse0x0A(ClientSocket cs)
        {
            cs.LoggedOn = true;
        }

        private static void Parse0x00(ClientSocket cs)
        {
            Builder builder = new Builder();
            builder.SendPacket(cs, 0x00);
        }

        internal static void Send0x50(ClientSocket cs)
        {
            byte[] byteOne = new byte[1] { 1 };

            SocketActions.SendData(cs, byteOne);

            Builder builder = new Builder();

            builder.InsertInt32(0);
            builder.InsertString("68XI");
            builder.InsertString(ReturnClient());
            builder.InsertInt32(0xD3);
            builder.InsertInt32(0);
            builder.InsertInt32(0);
            builder.InsertInt32(0);
            builder.InsertInt32(0);
            builder.InsertInt32(0);
            builder.InsertNTString("USA");
            builder.InsertNTString("United States");
            builder.SendPacket(cs, 0x50);

        }

        private static bool _starcraftClient;
        private static string ReturnClient()
        {
            GameClient gameClient = KryX2.Settings.UserSettings.GameClient;
            switch (gameClient)
            {
                case GameClient.Broodwar:
                    _starcraftClient = false;
                    break;
                case GameClient.Starcraft:
                    _starcraftClient = true;
                    break;
                case GameClient.Random:
                    _starcraftClient = !_starcraftClient;
                    break;
                default:
                    _starcraftClient = true;
                    break;
            }

            if (_starcraftClient)
                return "RATS";
            else
                return "PXES";
        }


        private static void Parse0x50(ClientSocket cs, Receiver receiver)
        {

            receiver.SkipLength(4); // Int32 logonType = receiver.GetInt32();
            cs.ServerToken = receiver.GetInt32();
            cs.ClientToken = receiver.GetInt32();
            receiver.SkipLength(8); // long mpqFiletime = receiver.GetInt64();
            string mpqFilename = receiver.GetNTString();
            byte[] checksumFormula = receiver.GetNTBytes();

            int crResult = -1, exeVer = -1, checkRevisionPass = 0;
            mpqFilename = mpqFilename.Substring(0,
                (mpqFilename.Length - 4)) + ".dll";

            byte[] exeInfo = CheckRevision.DoLockdownCheckRevision(checksumFormula,
                KryX2.Settings.GeneratedSettings.HashFiles,
                KryX2.Settings.GeneratedSettings.LockdownPath, mpqFilename, KryX2.Settings.GeneratedSettings.StarBin,
                ref exeVer, ref crResult);

            if (crResult == -1 || exeVer == -1)
            //if (checkRevisionPass != 1 || exeVer == 0)
            {
                //bad hashes perhaps
                Chat.Add(Color.Yellow, "CheckrevisionPass " + checkRevisionPass.ToString() + NewLine);
                Chat.Add(Color.White, "Exeversoin " + exeVer.ToString() + Environment.NewLine);
                //Chat.Add(Color.White, "No hashes or cr result is bad " + crResult + " / " + exeVer + NewLine);
                return;

            }


            CdKey cdkey;

            byte[] key1Hash;
            try
            {
                cdkey = new CdKey(cs.CdKey);
                key1Hash = cdkey.GetHash(cs.ClientToken, cs.ServerToken);

            }
            catch
            {
                Chat.Add(Color.Yellow, "Couldn't decode key." + NewLine);
                return;
            }

            Builder builder = new Builder();
            builder.InsertInt32(cs.ClientToken);
            builder.InsertInt32(exeVer);
            builder.InsertInt32(crResult);
            builder.InsertInt32(1); //keys used
            builder.InsertInt32(0); //spawn?
            builder.InsertInt32(13); //key length
            builder.InsertInt32(cdkey.Product);
            builder.InsertInt32(cdkey.Value1);
            builder.InsertInt32(0);
            builder.InsertByteArray(key1Hash);
            builder.InsertByteArray(exeInfo);
            builder.InsertByte(0);
            builder.InsertNTString(RandomName()); //in use by
            builder.SendPacket(cs, 0x51);

        }


        private static void Parse0x25(ClientSocket cs)
        {
            Builder builder = new Builder();
            builder.InsertInt32(0);
            builder.SendPacket(cs, 0x25);
        }


        private static void Parse0x51(ClientSocket cs, Receiver receiver)
        {
            Int32 keyResponse = receiver.GetInt32();
            string additionalMessage = receiver.GetNTString();

            switch (keyResponse)
            {
                case 0x000:
                    //removes plug
                    Builder builder = new Builder();
                    builder.InsertNonNTString("tenb");
                    builder.SendPacket(cs, 0x14);
                    //determine what type of name is being used and send packet accordingly
                    SendLogon(cs);
                    return;
                case 0x100:
                    Chat.Add(Color.White, "Old game version! (verbyte)" + NewLine);
                    return;
                case 0x101:
                    Chat.Add(Color.White, "Invalid game version!" + NewLine);
                    CDKeys.WriteBadCDKey(cs.CdKey, " game-version");
                    cs.CDKeyBlacklisted = true;
                    return;
                case 0x102:
                    Chat.Add(Color.White, "Must downgrade game version!" + NewLine);
                    return;
                case 0x200:
                    Chat.Add(Color.White, "Invalid cdkey!" + NewLine);
                    CDKeys.WriteBadCDKey(cs.CdKey, " invalid");
                    cs.CDKeyBlacklisted = true;
                    break;
                case 0x201:
                    Chat.Add(Color.White, "Key in use by: " + additionalMessage + NewLine);
                    break;
                case 0x202:
                    Chat.Add(Color.White, "Banned cdkey!" + NewLine);
                    CDKeys.WriteBadCDKey(cs.CdKey, " banned");
                    cs.CDKeyBlacklisted = true;
                    break;
                case 0x203:
                    Chat.Add(Color.White, "Cdkey meant for another product!" + NewLine);
                    break;
                default:
                    Chat.Add(Color.Yellow, "Unknown 0x51 response. sadface.com" + NewLine);
                    break;
            }

            SocketActions.ReconnectClient(cs, true, false);
        }

        private static void SendLogon(ClientSocket cs)
        {
            //sets name and password and sends appropriate packet types
            cs.AccountPass = UserSettings.ClientPassword;
            bool randomName = UserSettings.RandomNames;
            if (randomName)
            {
                cs.AccountName = RandomName();
                Send0x3D(cs);
            }
            else
            {
                cs.AccountName = UserSettings.ClientName;
                Send0x3A(cs);
            }
        }


        private static void Send0x3A(ClientSocket cs)
        {
            Builder builder = new Builder();
            builder.InsertInt32(cs.ClientToken);
            builder.InsertInt32(cs.ServerToken);
            builder.InsertByteArray(OldAuth.DoubleHashPassword(cs.AccountPass, cs.ClientToken, cs.ServerToken));
            builder.InsertNTString(cs.AccountName);
            builder.SendPacket(cs, 0x3A);
        }

        private static void Parse0x3A(ClientSocket cs, Receiver receiver)
        {
            Int32 accountResponse = receiver.GetInt32();

            switch (accountResponse)
            {
                case 0x000:
                    cs.FirstJoin = true;
                    //sets logged on variables
                    ConnectedBar.AddOne();
                    //confirm logon packet
                    Builder builder = new Builder();
                    builder.InsertNTString(cs.AccountName);
                    builder.InsertByte(0);
                    builder.SendPacket(cs, 0x0A);
                    //send join to targeted channel
                    Send0x0C(cs);
                    break;
                case 0x001:
                    //account doesnt exist. create account
                    Send0x3D(cs);
                    break;
                case 0x002:
                    //logon failed. generate random name, create account and so on
                    cs.AccountName = RandomName();
                    Send0x3D(cs);
                    break;
                default:
                    Chat.Add(Color.Wheat, "Unhandled 0x3A code." + NewLine);
                    break;

            }
        }

        private static void Send0x3D(ClientSocket cs)
        {
            Builder builder = new Builder();
            builder.InsertByteArray(OldAuth.HashPassword(cs.AccountPass));
            builder.InsertNTString(cs.AccountName);
            builder.SendPacket(cs, 0x3D);
        }

        private static void Parse0x3D(ClientSocket cs, Receiver receiver)
        {
            Int32 accountResponse = receiver.GetInt32();

            switch (accountResponse)
            {
                case 0x000:
                    //account creation successful
                    Send0x3A(cs);
                    break;
                default:
                    //creation failed, just try again, maybe name already in use who knows dont care to check
                    cs.AccountName = RandomName();
                    Send0x3D(cs);
                    break;
            }
        }

        private static void Parse0x0F(ClientSocket cs, Receiver receiver)
        {
            Int32 eventId = receiver.GetInt32();
            //Int32 flags = receiver.GetInt32();
            //Int32 ping = receiver.GetInt32();
            //receiver.SkipLength(12); //ip, acc#, regauth
            receiver.SkipLength(20); //^^ skipping all this

            string username, message;

            //bot joined channel
            if (eventId == 0x07)
            {
                receiver.GetNTString();
                //get channel joined
                message = receiver.GetNTString();
                //if channel is the void then key is bad.
                //first join, check if voided
                if (cs.FirstJoin)
                {
                    cs.FirstJoin = false;
                    //skip username
                    if (message == "The Void")
                    {
                        cs.CDKeyBlacklisted = true;
                        CDKeys.WriteBadCDKey(cs.CdKey, " voided");
                        //reconnect
                        SocketActions.ReconnectClient(cs, true, false);
                    }
                }
                //not first join
                else
                {
                    cs.Channel = message.ToLower();
                }
            }

            //if a whisper or talk check if from master
            if (eventId == 0x04 || eventId == 0x05)
            {
                username = receiver.GetNTString();
                if (username.ToLower().Equals(UserSettings.MasterName))
                {
                    message = receiver.GetNTString();
                    CommandBots(cs, message);
                }
            }
        }


        //r/ enter commands here
        private static void CommandBots(ClientSocket cs, string message)
        {
            string lowercaseMessage = message.ToLower();

            //5 in length or longer commands
            if (lowercaseMessage.Length > 4)
            {
                if (lowercaseMessage.Substring(0, 4) == "say ")
                {
                    Builder builder = new Builder();
                    builder.InsertNTString(message.Substring(4));
                    builder.SendPacket(cs, 0x0E);
                    return;
                }
                if (lowercaseMessage.Substring(0, 4) == "mov ")
                {
                    //change target
                    UserSettings.TargetChannel = message.Substring(4);
                    //send join
                    Builder builder = new Builder();
                    builder.InsertNTString("/join " + message.Substring(4));
                    builder.SendPacket(cs, 0x0E);
                    return;
                }
            }

            if (message == "online")
            {
                Builder builder = new Builder();
                builder.InsertNTString("We are " + ConnectedBar.ReturnConnected() + " of " + ConnectedBar.ReturnConfigured());
                builder.SendPacket(cs, 0x0E);
                return;
            }

            if (message == "bark")
            {
                Builder builder = new Builder();
                builder.InsertNTString("Woof");
                builder.SendPacket(cs, 0x0E);
                return;
            }

            if (message == "roll over")
            {
                Builder builder = new Builder();
                builder.InsertNTString("/me rolls over");
                builder.SendPacket(cs, 0x0E);
                return;
            }

            if (message == "who's a good bot?")
            {
                Builder builder = new Builder();
                builder.InsertNTString("I am, I'm a good bot!");
                builder.SendPacket(cs, 0x0E);
                return;
            }

        }
    }
}
