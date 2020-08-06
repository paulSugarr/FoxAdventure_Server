using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Data.SQLite;

namespace Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        public bool Logged { get; private set; }
        public string userName = "<unnamed>";
        private TcpClient client;
        public ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Login()
        {
            Stream = client.GetStream();
            // получаем имя пользователя

            while (true)
            {
                string message = GetMessage();
                if (ServerCommand.TryExecute(message, this))
                {
                    return;
                }
                if (DBWorking.ExistsUser(message))
                {
                    userName = message;
                    server.AnswerMessage("!login", Id);
                    while (true)
                    {
                        string password = GetMessage();
                        if (DBWorking.GetPassword(userName) == password)
                        {
                            server.AnswerMessage("!password", Id);
                            Logged = true;
                            message = userName + " залогинился";
                            server.BroadcastMessage(message, Id);
                            break;
                        }
                        else
                        {
                            Logged = false;
                            server.AnswerMessage("Неверный пароль", Id);
                            throw new Exception(string.Format("Неудачная попытка аутентификации под логином {0}", userName));
                        }
                    }
                    Console.WriteLine(message);
                    break;
                }
                else
                {
                    Logged = false;
                    server.AnswerMessage("Неверный логин, повторите:", Id);
                    throw new Exception(string.Format("Неудачная попытка идентификации под логином {0}", message));
                }
            }

        }


        public void Process()
        {
            try
            {
                Login();
                string message;
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        if (message.Length < 1) { throw new Exception("empty msg"); }
                        var logMessage = string.Format("{0}: {1}", userName, message);
                        Console.WriteLine(logMessage);

                        if (!ServerCommand.TryExecute(message, this))
                        {
                            server.AnswerMessage("Неверный синтаксис команды", Id);
                        }
                    }
                    catch
                    {
                        message = string.Format("{0}: разлогинился", userName);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}

