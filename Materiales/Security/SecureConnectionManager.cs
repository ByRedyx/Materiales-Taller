using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Materiales.Security
{
    public static class SecureConnectionManager
    {
        private static readonly string filePath = "db_connection.sec";
        private static readonly byte[] key = Encoding.UTF8.GetBytes("1234567890ABCDEF"); // 16 bytes (clave AES)
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("ABCDEF1234567890");  // 16 bytes (vector inicialización)

        public static void SaveConnectionString(string connString)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter sw = new StreamWriter(cs);
            sw.Write(connString);
        }

        public static string LoadConnectionString()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Archivo de conexión no encontrado.");

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using FileStream fs = new FileStream(filePath, FileMode.Open);
            using CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
