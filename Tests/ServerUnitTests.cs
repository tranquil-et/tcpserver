using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using tcpserver;

namespace Tests
{
    [TestClass]
    public class ServerUnitTests
    {
        [TestMethod]
        public void TestMessageSend()
        {
            int port = 8080;
            string testMsg = "Test message!";
            byte[] msg = Encoding.UTF8.GetBytes(testMsg);
            byte[] len = BitConverter.GetBytes(msg.Count());
            byte[] buffer = new byte[len.Length + msg.Length];

            len.CopyTo(buffer, 0);
            msg.CopyTo(buffer, len.Length);

            var server = new TcpServer();
            var waitHandle = new AutoResetEvent(false);
            var resultMsg = string.Empty;
            server.OnMessageReceived += (sender, args) =>
            {
                resultMsg = args.Message;
                waitHandle.Set();
            };

            server.Start(port);

            var client = new TcpClient("127.0.0.1", 8080);
            client.GetStream().Write(buffer, 0, buffer.Length);
            waitHandle.WaitOne();
            Assert.AreEqual(testMsg, resultMsg);
        }
    }
}
