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

                    int bytes = 0;
                    byte[] data = new byte[256];

                    do {
                        bytes = socketClient.Receive(data);
                    } while (socketClient.Available > 0);



                    Console.WriteLine($"Got file from client. Text in file: {stringBuilder.ToString()}");
                    File.WriteAllBytes(DateTime.Now.ToString().Replace('.','_').Replace(':', '-').Replace(' ', '_')+".txt", data);

                    socketClient.Send(Encoding.Unicode.GetBytes("File succesful got."));

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
