using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client 
    {
        Socket socket;
        IPEndPoint iPEndPoint;
        public Task process { get; private set; }
        public Client()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Client(string ip, int port)
        {
            try
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch(Exception)
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            process = new Task(ClientStartThread);
            process.Start();
        }
        private void ClientStartThread()
        {
            try
            {
                socket.Connect(iPEndPoint);
                Console.WriteLine($"Welcome to server[{iPEndPoint.Address}]. Enter names of files to send(enter ! to exit):");
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
                input.ForEach(x => FileSender(x));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }

        private void FileSender(string input)
        {
            if (File.Exists(input))
            {
                SendFileName(input);

                CheckAnswers(input);
            }
            else
            {
                Console.WriteLine("File name error.");
            }
        }

        private string ReceiveString(Socket socket)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];
            do
            {
                bytes = socket.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socket.Available > 0);
            return stringBuilder.ToString();
        }
        private void SendFileName(string input)
        {
            byte[] data = Encoding.Unicode.GetBytes(input);
            socket.Send(data);
        }
        private void SendFile(string input)
        {
            byte[] data = File.ReadAllBytes(input);

            socket.Send(data);
        }
        private void CheckAnswers(string input)
        {
            string answer = ReceiveString(socket);

            Console.WriteLine($"Got answer from server: {answer}.");

            if (answer == "Yes")
            {
                SendFile(input);
            }

            if (ReceiveString(socket) == "Good")
            {
                Console.WriteLine($"File {input} send ended.");
            }
            else
            {
                Thread.Sleep(100);
            }
        }

    }
}
