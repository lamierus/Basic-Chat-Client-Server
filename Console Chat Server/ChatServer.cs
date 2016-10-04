using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Basic_Console_Server {
    class Program {
        //static private TcpListener Listener;
        static void Main(string[] args) {
            try {
                //bool quit = false;
                // Initializes the Listener
                TcpListener listener = new TcpListener(IPAddress.Any, 8000);

                // Start Listeneting at the specified port
                listener.Start();

                Console.WriteLine("Server running - IP: " +
                    GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault()
                    + " Port: 8000");
                Console.WriteLine("Local end point:" + listener.LocalEndpoint);
                Console.WriteLine("Waiting for connections...");
                
                listener.BeginAcceptTcpClient(ClientThread, listener);

                do {
                    string input = Console.ReadLine();
                    if (input == "/quit") {
                        break;
                    }
                } while (true);
                if (listener.Pending()) {
                    listener.EndAcceptTcpClient();
                }
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

            do {
                try {
                    k = ns.Read(b, 0, 100);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    break;
                }
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++) {
                    chatMessage += Convert.ToChar(b[i]);
                }
                Console.WriteLine(chatMessage);
                
                // Test reply
                Byte[] replyData = Encoding.ASCII.GetBytes(DateTime.Now.ToString() + Environment.NewLine);
                ns.Write(replyData, 0, replyData.Length);
                ns.Flush();
            } while (chatMessage != "/quit") ;

            ns.Close();
            client.Close();
        }

        static private string[] GetAllLocalIPv4(NetworkInterfaceType _type) {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up) {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses) {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }
    }
}