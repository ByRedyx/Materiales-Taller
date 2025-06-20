using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GrupoOSG.Services
{
    public class SecurityService
    {
        private readonly byte[] key;
        private readonly byte[] iv;

        public SecurityService()
        {
            key = Encoding.UTF8.GetBytes("1029384756QPWOEI");
            iv = Encoding.UTF8.GetBytes("ZMXNCB5647382910");
        }

        public SecurityService(string keyString, string ivString)
        {
            key = Encoding.UTF8.GetBytes(keyString);
            iv = Encoding.UTF8.GetBytes(ivString);

            if (key.Length != 16 || iv.Length != 16)
            {
                throw new ArgumentException("La clave y el IV deben tener exactamente 16 bytes.");
            }
        }

        // Encripta string → byte[]
        public byte[] EncryptString(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);
            sw.Write(plainText);
            sw.Flush();
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        // Desencripta byte[] → string
        public string DecryptToString(byte[] cipherData)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using MemoryStream ms = new(cipherData);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }

        // Encripta byte[] → byte[]
        public byte[] EncryptBytes(byte[] data)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        // Desencripta byte[] → byte[]
        public byte[] DecryptBytes(byte[] encryptedData)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using MemoryStream ms = new(encryptedData);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream result = new();
            cs.CopyTo(result);
            return result.ToArray();
        }
    }
}
