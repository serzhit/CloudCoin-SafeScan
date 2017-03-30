using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace CloudCoin_SafeScan
{
    public static class Utils
    {
        private const int Rfc2898KeygenIterations = 100;
        private const int AesKeySizeInBits = 128;

        public static string ChooseInputFile()
        {
            OpenFileDialog FD = new OpenFileDialog();
            FD.Multiselect = true;
            FD.Title = "Choose file with Cloudcoin(s)";
            FD.InitialDirectory = @"C:\Users\Sergey\Documents\GitHub\CloudCoinFoundation\Bank";
            if (FD.ShowDialog() == true)
            {
                return FD.FileName;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

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

        public static byte[] Encrypt(string tobeEncrypted, string password, byte[] salt)
        {
            /*            int reminder = tobeEncrypted.Length % 16;
                        if ( reminder != 0 )
                        {
                            char[] spaces = new char[16 - reminder];
                            for (int i = 0; i < (16 - reminder); i++) spaces.SetValue((char)32, i);
                            tobeEncrypted = tobeEncrypted + new string(spaces);
                        }
            */
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
