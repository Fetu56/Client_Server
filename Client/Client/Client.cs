using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        public static string ip { get; private set; } = "127.0.0.1";
        public static int port { get; private set; } = 8000;
        Socket socket;
        IPEndPoint iPEndPoint;
        public Client()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            try
            {
                socket.Connect(iPEndPoint);
                Console.WriteLine($"Welcome to server[{ip}]. Enter names of files to send(enter ! to exit):");
                List<string> input = new List<string>();
                string temp = "";
                while (true)
                {
                    temp = Console.ReadLine();
                    if (temp != "!")
                        input.Add(temp);
                    else
                        break;
                }
                input.ForEach(x => Get(x));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }

        private void Get(string input)
        {
            if (File.Exists(input))
            {
                int bytes = 0;
                byte[] data = new byte[256];
                data = Encoding.Unicode.GetBytes(input);
                socket.Send(data);
                StringBuilder stringBuilder = new StringBuilder();
                do
                {
                    bytes = socket.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socket.Available > 0);
                Console.WriteLine($"Got answer from server: {stringBuilder.ToString()}");
                if (stringBuilder.ToString() == "Yes")
                {
                    data = File.ReadAllBytes(input);

                    socket.Send(data);
                }
                stringBuilder.Clear();
                bytes = 0;
                data = new byte[256];
                do
                {
                    bytes = socket.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socket.Available > 0);
                if (stringBuilder.ToString() == "Good")
                {
                    Console.WriteLine($"File {input} send ended.");
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            else
            {
                Console.WriteLine("Введёно имя неверного файла");
            }
        }
    }
}
