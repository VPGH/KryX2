using System;
using System.Net;
using System.Net.Sockets;
using KryX2.PacketManagement;
using KryX2.FileManagement;

namespace KryX2.Client
{


    internal class ClientData
    {

        internal string Channel { get; set; }
        internal bool FirstJoin { get; set; }
        internal bool Active { get; set; }
        internal bool ProxyAuthorized { get; set; }
        internal IPAddress ProxyAddress { get; set; }
        internal int ProxyPort { get; set; }
        internal ProxyType ProxyFormat { get; set; }
        public bool LoggedOn { get; set; }
        public bool Finalized { get; set; }
        internal bool CDKeyBlacklisted { get; set; }

        //logon requirements
        internal string AccountName { get; set; }
        internal string AccountPass { get; set; }
        internal string CdKey { get; set; }
        internal Int32 ServerToken { get; set; }
        internal Int32 ClientToken { get; set; }

        //custom data receiver/manager
        internal PacketBuffer PacketBuffer = new PacketBuffer();

    }





}