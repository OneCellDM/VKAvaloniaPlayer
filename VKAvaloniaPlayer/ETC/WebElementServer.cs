using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace WebElement
{
    public class WebElementServer
    {
        public delegate void Message(string Data);
        public delegate void Error(Exception ex);
        public event Message MessageRecived;
        public event Error ErrorEvent;


        Thread ServerThread { get; set; }
        public int Port { get; private set; }

        public TcpListener TcpListener { get; set; }
        public TcpClient Client { get; set; }
        public WebElementServer(int port) => this.Port = port;
        public bool ServerStarted { get; set; }
        public void StartServerOnThread()
        {
            if (ServerStarted)
                return;

            ServerThread = new Thread(() => StartServer());
            ServerThread.Start();
        }
        public void StartServer()
        {
            if (ServerStarted) return;

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
                TcpListener = new TcpListener(ipPoint);
                TcpListener?.Start();
                Console.WriteLine("Server started on port {0}", Port);
                ServerStarted = true;

                ReadConnections();
            }
            catch (Exception ex)
            {
                ServerStarted = false;
                ErrorEvent?.Invoke(ex);
            }

        }
        public bool Stop()
        {
            try
            {
                Client?.Close();
                Client?.Dispose();
                TcpListener?.Stop();


                ServerStarted = false;
                Console.WriteLine("Stopped");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void ReadConnections()
        {

            while (ServerStarted)
            {
                try
                {
                    Client = TcpListener.AcceptTcpClient();
                    Console.WriteLine("Client Connected");
                    while (true)
                    {
                        byte[] data = new byte[256];

                        StringBuilder response = new StringBuilder();

                        var stream = Client.GetStream();

                        do
                        {

                            int bytes = stream.Read(data, 0, data.Length);
                            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);


                        MessageRecived?.Invoke(response.ToString());
                    }



                }
                catch (Exception ex)
                {
                    ServerStarted = false;
                    ErrorEvent?.Invoke(ex);

                }
            }


        }

    }
}