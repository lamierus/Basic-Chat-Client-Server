using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Basic_Console_Server {
    class Program {
        //static private TcpListener Listener;
        static void Main(string[] args) {
            try {
                bool quit = false;
                // Initializes the Listener
                TcpListener listener = new TcpListener(IPAddress.Any, 8000);

                // Start Listeneting at the specified port
                listener.Start();

                Console.WriteLine("Server running - Port: 8000");
                Console.WriteLine("Local end point:" + listener.LocalEndpoint);
                Console.WriteLine("Waiting for connections...");

                /*Socket sock = Listener.AcceptSocket();
                // When accepted
                Console.WriteLine("Connection accepted from " + sock.RemoteEndPoint);

                byte[] b = new byte[100];
                int k = sock.Receive(b);
                Console.WriteLine("Recieved...");

                for (int i = 0; i < k; i++) {
                    Console.Write(Convert.ToChar(b[i]));
                }

                ASCIIEncoding asen = new ASCIIEncoding();
                sock.Send(asen.GetBytes("Automatic message: " + "String received by server!"));
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Automatic message sent!");

                sock.Close();*/
                listener.BeginAcceptTcpClient(ClientThread, listener);
                do {
                    string input = Console.ReadLine();
                    if (input == "/quit") {
                        quit = true;
                    }
                } while (!quit);

                listener.Stop();
            } catch (Exception e) {
                Console.WriteLine("Error:" + Environment.NewLine + e.StackTrace);
            }
            Console.ReadLine();
        }

        static private void ClientThread(IAsyncResult res) {
            TcpListener listener = (TcpListener)res.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(res);

            Console.WriteLine("Connection accepted from " + client.Client.RemoteEndPoint);
            
            string chatMessage = "";
            byte[] b = new byte[100];
            int k;
            NetworkStream ns = client.GetStream();
            while (chatMessage != "/quit") {
                k = ns.Read(b, 0, 100);
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++) {
                    chatMessage += Convert.ToChar(b[i]);
                }
                Console.WriteLine(chatMessage);
                
                // Test reply
                Byte[] replyData = Encoding.ASCII.GetBytes(DateTime.Now.ToString() + Environment.NewLine);
                ns.Write(replyData, 0, replyData.Length);
            } 
            ns.Flush();
            ns.Close();
            client.Close();
        }
    }
}