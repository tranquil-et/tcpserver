using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tcpserver
{
    public class TcpServer
    {
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        public void Start(int port)
        {            
            try
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.BeginAcceptTcpClient(OnClientAccepted, listener);                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnClientAccepted(IAsyncResult result)
        {
            if (!(result.AsyncState is TcpListener listener))
            {
                return;
            }

            try
            {
                var context = new ClientContext();
                context.Client = listener.EndAcceptTcpClient(result);
                context.Stream = context.Client.GetStream();
                context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, OnClientRead, context);
            }
            finally
            {
                listener.BeginAcceptTcpClient(OnClientAccepted, listener);
            }
        }

        void OnClientRead(IAsyncResult result)
        {
            if (!(result.AsyncState is ClientContext context))
            {
                return;
            }

            try
            {
                int read = context.Stream.EndRead(result);
                //context.Message.Write(context.Buffer, 0, read);

                int length = BitConverter.ToInt32(context.Buffer, 0);
                byte[] buffer = new byte[1024];

                while (length > 0)
                {
                    read = context.Stream.Read(buffer, 0, Math.Min(buffer.Length, length));
                    context.Message.Write(buffer, 0, read);
                    length -= read;
                }

                OnMessageReceived.Invoke(this, 
                    new MessageReceivedEventArgs(Encoding.UTF8.GetString(context.Message.ToArray())));
            }
            catch (Exception)
            {
                context.Client.Close();
                context.Stream.Dispose();
                context.Message.Dispose();
                context = null;
            }
            finally
            {
                if (context != null)
                    context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, OnClientRead, context);
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
