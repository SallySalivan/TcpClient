using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine("Добро пожаловать, {0}", userName);
                Console.WriteLine("\nВопросы для чата : " +
                    "\n1.Как дела ? " +
                    "\n2.Как настроение? " +
                    "\n3.Что делаешь? " +
                    "\n4.<.CountUser> пользователи " +
                    "\n5.Команда <Пока> закрывает чат. " +
                    "\n6.Команда написать всем </All сообщение> " +
                    "\n7.Команда написать определеному пользователю </privat (Ник) сообщение)" +
                    "\n");
                
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {

            Console.WriteLine("Введите сообщение: ");
            
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                if (message == "Пока")
                    Disconnect();
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[128];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0);
        }
    }
}
