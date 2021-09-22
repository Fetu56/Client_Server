namespace Client
{
    class Program {
        static void Main()
        {
            Client client = new Client();
            client.Start();

            client.process.Wait();
        }
    }
}
