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
        Socket socket;
        IPEndPoint iPEndPoint;
        public Server()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Server(string ip, int port)
        {
            try
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch (Exception)
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            Console.WriteLine($"Start server [{iPEndPoint.Address}]...");
            try
            {
                socket.Bind(iPEndPoint);
                socket.Listen(10);

                while (true)
                {
                    Connected();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void Connected()
        {
            Socket socketClient = socket.Accept();
            Console.WriteLine("Registrated new user.");
            try
            {
                while (true)
                {
                    string filename = ReceiveString(socketClient); ;
                    Console.WriteLine($"Got file name from client. Name of file: {filename}.");

                    socketClient.Send(Encoding.Unicode.GetBytes("Yes"));


                    File.WriteAllBytes(filename, ReceiveListData(socketClient).ToArray());

                    socketClient.Send(Encoding.Unicode.GetBytes("Good"));
                }
            }
            catch(Exception)
            {}


            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
        }

        private string ReceiveString(Socket socketClient)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            byte[]  data = new byte[256];
            do
            {
                bytes = socketClient.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socketClient.Available > 0);
            return stringBuilder.ToString();
        }   

        private List<byte> ReceiveListData(Socket socketClient)
        {
            byte[] data = new byte[256];
            List<byte> listData = new List<byte>();
            do
            {
                socketClient.Receive(data);
                listData.AddRange(data);
            } while (socketClient.Available > 0);
            return listData;
        }
    }
}
