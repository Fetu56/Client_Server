using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        public static string ip { get; private set; } = "127.0.0.1";
        public static int port { get; private set; } = 8000;
        Socket socket;
        IPEndPoint iPEndPoint;
        public Server()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            Console.WriteLine("Start server...");
            try
            {
                socket.Bind(iPEndPoint);
                socket.Listen(10);

                while (true)
                {
                    Socket socketClient = socket.Accept();
                    Console.WriteLine("Registrated new user.");
                    StringBuilder stringBuilder = new StringBuilder();

                    int bytes;
                    byte[] data;
                    while (true)
                    {
                        stringBuilder.Clear();
                        bytes = 0;
                        data = new byte[256];
                        do
                        {
                            bytes = socketClient.Receive(data);
                            stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (socketClient.Available > 0);



                        string filename = stringBuilder.ToString();
                        Console.WriteLine($"Got file name from client. Name of file: {filename}");

                        socketClient.Send(Encoding.Unicode.GetBytes("Yes"));

                        bytes = 0;
                        data = new byte[256];
                        List<byte> listData = new List<byte>();
                        stringBuilder.Clear();


                        do
                        {
                            bytes = socketClient.Receive(data);
                            for (int i = 0; i < data.Length; i++)
                            {
                                listData.Add(data[i]);
                            }
                            stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (socketClient.Available > 0);

                        File.WriteAllBytes(filename, listData.ToArray());

                        socketClient.Send(Encoding.Unicode.GetBytes("Good"));
                    }


                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
