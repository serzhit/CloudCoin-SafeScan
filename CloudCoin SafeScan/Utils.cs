using System;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace CloudCoin_SafeScan
{
    public static class Utils
    {
        private const int Rfc2898KeygenIterations = 100;
        private const int AesKeySizeInBits = 128;

        public static string ToHexString(byte[] digest)
        {
            String hash = "";
            foreach (byte aux in digest)
            {
                int b = aux & 0xff;
                if (b.ToString("X2").Length == 1) hash += "0";
                hash += b.ToString("X2");
            }
            return hash;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static int Denomination2Int(CloudCoin.Denomination d)
        {
            switch (d)
            {
                case CloudCoin.Denomination.One: return 1;
                case CloudCoin.Denomination.Five: return 5;
                case CloudCoin.Denomination.Quarter: return 25;
                case CloudCoin.Denomination.Hundred: return 100;
                case CloudCoin.Denomination.KiloQuarter: return 250;
                default: return 0;
            }
        }

        public static byte[] Encrypt(string tobeEncrypted, string password, byte[] salt)
        {
            byte[] cipherText;
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int KeyStrengthInBytes = aes.KeySize / 8;
                Rfc2898DeriveBytes rfc2898 =
                    new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(cs))
                        {
                            swEncrypt.Write(tobeEncrypted);
                        }
                        cipherText = ms.ToArray();
                    }
                }
            }
            return cipherText;
        }

        public static string Decrypt(byte[] encryptedBytes, string password, byte[] salt)
        {
            string plainText = null;
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int KeyStrengthInBytes = aes.KeySize / 8;
                Rfc2898DeriveBytes rfc2898 =
                    new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(cs))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }
    }
}
