using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tcpserver
{
    public class ClientContext
    {
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        public byte[] Buffer { get; set; } = new byte[4];
        public MemoryStream Message { get; set; } = new MemoryStream();
    }
}
