using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using PublicKeyEncryption;
using LanguageExt;
using System.Numerics;
using System.ComponentModel;

namespace Guide0Server
{
    class Program
    {
        #region Variables
        // Encryption
        static readonly Encryption encryption = new Encryption();
        static readonly RSAEncryption RSAEncryption = new RSAEncryption();

        // Key
        static string key;

        // Private key
        static long a;

        // Client list
        public static List<TcpClient> clients = new List<TcpClient>();
        #endregion

        static void Main(string[] args)
        {
            // Setup connection
            int port = 13356;
            IPAddress ip = IPAddress.Any;
            IPEndPoint localEndpoint = new IPEndPoint(ip, port);
            TcpListener listener = new TcpListener(localEndpoint);
            listener.Start();

            Console.WriteLine("Waiting for client");

            a = new Random().Next(1000000, 9999999);

            AcceptClients(listener);

            while (true)
            {
                Console.WriteLine("Write something to clients");
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].GetStream().Write(encryption.Encrypt(buffer, key), 0, buffer.Length);
                }

                if (message.ToLower() == "c")
                {
                    Console.WriteLine("Connection closed");
                    break;
                }

            }
        }

        /// <summary>
        /// Async function, running for receiving messages at all time
        /// </summary>
        /// <param name="stream">Stream needed for knowing location to fetch messages</param>
        /// <param name="fromMessage">Prefix for received messages</param>
        public static async void RecieveMessages(NetworkStream stream, string fromMessage)
        {
            while (true)
            {
                byte[] buffer = new byte[256];
                int nBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(encryption.Decrypt(buffer, key), 0, nBytesRead);

                Console.WriteLine($"{fromMessage} {message}");
                if (message.ToLower() == "c")
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        public static async void AcceptClients(TcpListener listener)
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(client);

                NetworkStream stream = client.GetStream();
                key = RSAEncryption.ExchangeKey(stream, a);
                RecieveMessages(stream, "Client writes:");
            }
        }
    }
}
