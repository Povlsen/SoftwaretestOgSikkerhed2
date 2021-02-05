using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PublicKeyEncryption
{
    public class Encryption
    {
        public byte key = 69;
        public Encryption()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("a");
            EncryptByte(bytes);
            DecryptByte(bytes);
        }

        public byte[] EncryptByte(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                int change = i + key;
                while (change + bytes[i] > 255) change -= 256;
                bytes[i] += (byte)change;
            }
            return bytes;
        }

        public byte[] DecryptByte(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                int change = i + key;
                while (change - bytes[i] < 0) change += 256;
                bytes[i] -= (byte)change;
            }
            return bytes;
        }

        public byte[] Encrypt(byte[] bytes, string key)
        {
            int length = key.Length / 3;
            byte[] keys = new byte[3];
            keys[0] = (byte)(int.Parse(key.Substring(0, length)) % 255);
            keys[1] = (byte)(int.Parse(key.Substring(length, length*2)) % 255);
            keys[2] = (byte)(int.Parse(key.Substring(length*2)) % 255);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 5 == 0)
                {
                    bytes[i] += (byte)keys[0];
                }
                else if (i % 3 == 0)
                {
                    bytes[i] += (byte)keys[1];
                } else
                {
                    bytes[i] += (byte)keys[2];
                }
            }
            return bytes;
        }

        public byte[] Decrypt(byte[] bytes, string key)
        {
            int length = key.Length / 3;
            byte[] keys = new byte[3];

            keys[0] = (byte)(int.Parse(key.Substring(0, length)) % 255);
            keys[1] = (byte)(int.Parse(key.Substring(length, length * 2)) % 255);
            keys[2] = (byte)(int.Parse(key.Substring(length * 2)) % 255);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 5 == 0)
                {
                    bytes[i] -= (byte)keys[0];
                }
                else if (i % 3 == 0)
                {
                    bytes[i] -= (byte)keys[1];
                }
                else
                {
                    bytes[i] -= (byte)keys[2];
                }
            }
            return bytes;
        }
    }

    public class RSAEncryption
    {
        public int n = 5101;
        public int g = 11;

        public string ExchangeKey(NetworkStream stream, long personalKey)
        {
            byte[] keyBuffer = BigInteger.ModPow(g, personalKey, n).ToByteArray();
            stream.Write(keyBuffer, 0, keyBuffer.Length);

            byte[] buffer = new byte[256];
            int nBytesRead = stream.Read(buffer, 0, buffer.Length);

            string key = AddOwnKey(new BigInteger(buffer), personalKey);
            return key;
        }

        public string AddOwnKey(BigInteger receivedPubKey, long personalKey)
        {
            var key = BigInteger.ModPow(receivedPubKey, personalKey, n);
            
            return key.ToString();
        }

    }
}
