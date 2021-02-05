using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Non_Parametric_Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "Hej med Dig";
            byte[] bytes = Encoding.UTF8.GetBytes(message);

            // Encrypt
            bytes = (encryptMessage(bytes, false));
            message = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(message);

            // Descrypt
            bytes = (encryptMessage(bytes, true));
            message = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(message);

            Console.ReadLine();
        }

        public static byte[] encryptMessage(byte[] buffer, bool isEncrypted)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (isEncrypted)
                    buffer[i] -= 4;
                else
                    buffer[i] += 4;
            }
            return buffer;
        }
    }
}
