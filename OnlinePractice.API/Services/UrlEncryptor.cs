using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace OnlinePractice.API.Services
{


    public class UrlEncryptor
    {
        private static readonly byte[] Key = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        //private static readonly byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static string EncryptUrl(string url)
        {
            try
            {

                byte[] encryptedBytes;
                byte[] iv;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.GenerateIV(); // Generate a new IV

                    iv = aesAlg.IV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(url);
                            }

                            encryptedBytes = msEncrypt.ToArray();
                        }
                    }
                }

                // Combine IV and encrypted data
                byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
                Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

                return Convert.ToBase64String(combinedBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string DecryptUrl(string encryptedUrl)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedUrl);

            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];

            Buffer.BlockCopy(combinedBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            string decryptedUrl = string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedUrl = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedUrl;
        }
    }

}
