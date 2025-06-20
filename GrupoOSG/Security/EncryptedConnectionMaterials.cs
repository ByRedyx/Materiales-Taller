using GrupoOSG.Services;
using System.IO;

namespace GrupoOSG.Security
{
    public static class EncryptedConnectionMaterials
    {
        private static readonly string filePath = "materials_connection_view.sec";
        private static readonly SecurityService security = new SecurityService(
            "1234567890ABCDEF", // Key 16 bytes
            "ABCDEF1234567890"  // IV 16 bytes
        );

        public static void SaveConnectionString(string connString)
        {
            byte[] encrypted = security.EncryptString(connString);
            File.WriteAllBytes(filePath, encrypted);
        }

        public static string LoadConnectionString()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Archivo de conexión no encontrado.");
            }

            byte[] encrypted = File.ReadAllBytes(filePath);
            return security.DecryptToString(encrypted);
        }
    }
}
