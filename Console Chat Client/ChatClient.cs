using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace Basic_Console_Client {
    class Program {
        static void Main(string[] args) {
            try {
                TcpClient tcpclnt = new TcpClient();
                for (int i = 0; i < 10; i++) {
                    Console.WriteLine("Connecting.....");
                    try {
                        tcpclnt.Connect(IPAddress.Loopback, 8000);
                        // Use the ipaddress as in the server program
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    if (tcpclnt.Connected) {
                        break;
                    }

                    Console.WriteLine("Waiting 5 Seconds before trying again.");
                    DateTime start = DateTime.Now;
                    while (DateTime.Now.Subtract(start).Seconds < 5) { }
                    Console.WriteLine();
                }

                Console.WriteLine("Connected...");
                Console.Write("Enter the string to be sent: ");

                string str = Console.ReadLine();
                while (str != "/quit") {
                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Sending...");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    for (int i = 0; i < k; i++) {
                        Console.Write(Convert.ToChar(bb[i]));
                    }
                    str = Console.ReadLine();
                }
                tcpclnt.Close();

            } catch (Exception e) {
                Console.WriteLine("Error:" + Environment.NewLine + e.StackTrace);
            }

            //Console.ReadLine();
        }
    }
}