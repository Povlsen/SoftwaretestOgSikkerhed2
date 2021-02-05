using PublicKeyEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Guide0Client
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
        static long b;
        #endregion

        static void Main(string[] args)
        {
            // Setup connection
            TcpClient client = new TcpClient();
            int port = 13356;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            client.Connect(endPoint);
            NetworkStream stream = client.GetStream();

            Console.WriteLine("Write something: ");

            b = new Random().Next(1000000, 9999999);

            // Setup secure connection
            key = RSAEncryption.ExchangeKey(stream, b);
            RecieveMessage(stream, "Server writes:");

            while (true)
            {
                string userInput = Console.ReadLine();
                if (userInput.ToLower() == "c")
                {
                    Console.WriteLine("Connection closed");
                    break;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(userInput);
                buffer = encryption.Encrypt(buffer, key);
                stream.Write(buffer, 0, buffer.Length);
            }
            client.Close();
        }

        /// <summary>
        /// Async function, running for receiving messages at all time
        /// </summary>
        /// <param name="stream">Stream needed for knowing location to fetch messages</param>
        /// <param name="fromMessage">Prefix for received messages</param>
        public static async void RecieveMessage(NetworkStream stream, string fromMessage)
        {
            while (true)
            {
                byte[] buffer = new byte[256];
                int nBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(encryption.Decrypt(buffer, key), 0, nBytesRead);

                if (message != string.Empty)
                {
                    Console.WriteLine($"{fromMessage} {message}");
                }
            }
        }

        
    }
}
