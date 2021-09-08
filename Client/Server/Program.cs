using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server {
    class Program {
        static int port = 8000;
        static void Main(string[] args) {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Start server...");
            try {
                socket.Bind(iPEndPoint);
                socket.Listen(10);

                while (true) {
                    Socket socketClient = socket.Accept();

                    StringBuilder stringBuilder = new StringBuilder();

                    int bytes = 0;
                    byte[] data = new byte[256];

                    do {
                        bytes = socketClient.Receive(data);
                        stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socketClient.Available > 0);

                    Console.WriteLine($"Got data {Encoding.Unicode.GetString(data)} form client, answer sended.");
                    switch(Encoding.Unicode.GetString(data).Split(',')[2].ToCharArray()[0]) {
                        case '+':
                            data = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(data).Split(',')[0] + Encoding.Unicode.GetString(data).Split(',')[2] + Encoding.Unicode.GetString(data).Split(',')[1] + " = " + (int.Parse(Encoding.Unicode.GetString(data).Split(',')[0]) + int.Parse(Encoding.Unicode.GetString(data).Split(',')[1])).ToString());
                            break;
                        case '-':
                            data = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(data).Split(',')[0] + Encoding.Unicode.GetString(data).Split(',')[2] + Encoding.Unicode.GetString(data).Split(',')[1] + " = " + (int.Parse(Encoding.Unicode.GetString(data).Split(',')[0]) - int.Parse(Encoding.Unicode.GetString(data).Split(',')[1])).ToString());
                            break;
                        case '*':
                            data = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(data).Split(',')[0] + Encoding.Unicode.GetString(data).Split(',')[2] + Encoding.Unicode.GetString(data).Split(',')[1] + " = " + (int.Parse(Encoding.Unicode.GetString(data).Split(',')[0]) * int.Parse(Encoding.Unicode.GetString(data).Split(',')[1])).ToString());
                            break;
                        case '/':
                            data = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(data).Split(',')[0] + Encoding.Unicode.GetString(data).Split(',')[2] + Encoding.Unicode.GetString(data).Split(',')[1] + " = " + (int.Parse(Encoding.Unicode.GetString(data).Split(',')[0]) / int.Parse(Encoding.Unicode.GetString(data).Split(',')[1])).ToString());
                            break;
                    }
                    
                    socketClient.Send(data);

                    //socketClient.Shutdown(SocketShutdown.Both);
                    //socketClient.Close();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
