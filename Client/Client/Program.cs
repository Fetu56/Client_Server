using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client {
    class Program {
        static string ip = "127.0.0.1";
        static int port = 8000;

        static void Main(string[] args) {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                socket.Connect(iPEndPoint);
                int bytes = 0;
                byte[] data = new byte[256];

                data = Encoding.Unicode.GetBytes(Console.ReadLine());
                socket.Send(data);
                    StringBuilder stringBuilder = new StringBuilder();
                while (true) {

                    

                    do {
                        bytes = socket.Receive(data);
                        stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);
                    Console.WriteLine($"Got answer from server: {stringBuilder.ToString()}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
