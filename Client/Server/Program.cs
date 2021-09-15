using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        static string GetWordsCount(string getStr)
        {
            string retStr = new string("Words:");
            List<KeyValuePair<int, string>> words = new List<KeyValuePair<int, string>>();
            getStr.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => {
                if (words.Where(y => y.Value == x.ToLower()).Count() > 0)
                    words[words.IndexOf(words.Find(y => y.Value == x.ToLower()))] = new KeyValuePair<int, string>(words.Find(y => y.Value == x.ToLower()).Key + 1, x.ToLower());
                else
                    words.Add(new KeyValuePair<int, string>(1, x.ToLower()));
            });


            words.ForEach(x => retStr += $" {x.Value} - {x.Key},");
            return retStr;
        }
    }
}
