using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CloudCoin_SafeScan
{
    public static class Convert
    {
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

        public static byte[] Encrypt(byte[] bytesequence, string password, byte[] salt)
        {
            int Rfc2898KeygenIterations = 100;
            int AesKeySizeInBits = 128;
            byte[] cipherText = null;
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int KeyStrengthInBytes = aes.KeySize / 8;
                Rfc2898DeriveBytes rfc2898 =
                    new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesequence, 0, bytesequence.Length);
                    }
                    cipherText = ms.ToArray();
                }
                return cipherText;
            }
        }

        public static byte[] Decrypt(byte[] bytesequence, string password, byte[] salt)
        {
            int Rfc2898KeygenIterations = 100;
            int AesKeySizeInBits = 128;
            byte[] plainText = null;
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int KeyStrengthInBytes = aes.KeySize / 8;
                Rfc2898DeriveBytes rfc2898 =
                    new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesequence, 0, bytesequence.Length);
                    }
                    plainText = ms.ToArray();
                }
                return plainText;
            }
        }
    }
}
