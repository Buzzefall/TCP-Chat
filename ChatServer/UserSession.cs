﻿using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using DataPackaging;
using System.Runtime.Serialization.Formatters.Binary;
using DataPackaging.Interfaces;

namespace ChatServer
{
    internal class UserSession : IUserSession, IDisposable
    {
        protected internal bool IsAuthorized { get; set; }
        protected internal string Login { get; set; }
        protected internal string Password { get; set; }
        protected internal string Name { get; set; }
        protected internal string UserId { get; set; }

        protected internal TcpClient Client { get; private set; }
        protected internal NetworkStream Stream { get; private set; }

        ~UserSession()
        {
            Dispose();
        }

        public UserSession(TcpClient client)
        {
            Client = client;
            if (client != null)
                Stream = client.Connected ? client.GetStream() : null;
            else
                Stream = null;

            IsAuthorized = false;
            Login = null;
            Password = null;
            Name = null;
            UserId = null;
        }

        public void Dispose()
        {
            Stream?.Close();
            Client?.Close();
        }

        public void Start()
        {
            //Проверка ошибки создания Client'a
            try
            {
                if (Client == null)
                    throw new Exception("Client is null! (UserSession.Start() Error)");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Отправка сообщения в открытый поток
        public void SendMessage(IBinarySerializable message)
        {
            try
            {
                if (Client.Connected)
                {
                    message.SerializeBytes(Stream);
                }
            }

            catch (IOException e)
            {
                if (e.Message == "Unable to write data to the transport connection: An established connection was aborted by the software in your host machine.")
                    Console.WriteLine($"[System] (SendMessage -> {Name}) {e.Message}");
            }

            catch (Exception e)
            {
                Console.WriteLine($"[System] Some serious problems at SendMessage(IBinarySerializable message): {e.Message}");
            }
        }

        //Получение сообщения из потока
        public object ReceiveMessage()
        {
            try
            {
                if (Stream.DataAvailable)
                {
                    //Десериализуем объект-сообщение из потока
                    BinaryFormatter binform = new BinaryFormatter();
                    return binform.Deserialize(Stream);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public string PrintInfo()
        {
            string output = "UserID: " + UserId + Environment.NewLine +
                            "User Name: " + Name + Environment.NewLine +
                            "Login: " + Login + Environment.NewLine +
                            "Password: " + Password + Environment.NewLine + "----------------" + Environment.NewLine;

            Console.WriteLine(output);

            return output;
        }
    }
}
