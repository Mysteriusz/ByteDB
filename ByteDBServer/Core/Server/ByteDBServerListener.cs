using System;
using System.Net;
using System.Net.Sockets;

namespace ByteDBServer.Core.Server
{
    internal class ByteDBServerListener
    {
        public static TcpListener Listener { get; private set; }
        
        public ByteDBServerListener(string address, int port)
        {
            Listener = new TcpListener(IPAddress.Parse(address), port);
        }
    }
}
