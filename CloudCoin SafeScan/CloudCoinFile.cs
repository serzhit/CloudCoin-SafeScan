using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    class CloudCoinFile
    {
        public enum Type { json, jpeg, unknown }
        public Type Filetype;
        string Filename;
        FileInfo FI;
        public CoinStack Coins;

        public CloudCoinFile(string fullPath)
        {
            FI = new FileInfo(fullPath);
            if (FI.Exists)
            {
                Filename = fullPath;
                using (Stream fsSource = FI.Open(FileMode.Open))
                {
                    byte[] signature = new byte[20];
                    fsSource.Read(signature, 0, 20);
                    string sig = Encoding.UTF8.GetString(signature);
                    var reg = new Regex(@"{[.\n\t\s\x09\x0A\x0D]*""cloudcoin""");
                    if (Enumerable.SequenceEqual(signature.Take(3), new byte[] { 255, 216, 255 })) //JPEG
                    {
                        Filetype = Type.jpeg;
                        var coin = ReadJpeg(fsSource);
                        Coins = new CoinStack(coin);
                    }
                    else if (reg.IsMatch(sig)) //JSON
                    {
                        Filetype = Type.json;
                        Coins = ReadJson(fsSource);
                    }
                }
                var newFileName = FI.FullName + ".imported";
                File.Move(FI.FullName, newFileName);
            }
            else
            {
                throw   new FileNotFoundException();
            }            
        }

        private CloudCoin ReadJpeg(Stream jpegFS)
        {
            // TODO: catch exception for wrong file format
            //            filetype = Type.jpeg;
            byte[] fileByteContent = new byte[455];
            int numBytesToRead = fileByteContent.Length;
            int numBytesRead = 0;
            string[] an = new string[RAIDA.NODEQNTY];
            string[] aoid = new string[1];
            int sn;
            int nn;
            string ed;

            jpegFS.Position = 0;
            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = jpegFS.Read(fileByteContent, numBytesRead, numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

            string jpegHexContent = "";
            jpegHexContent = Utils.ToHexString(fileByteContent);

            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                an[i] = jpegHexContent.Substring(40 + i * 32, 32);
            }
            aoid[0] = jpegHexContent.Substring(840, 55);
            ed = jpegHexContent.Substring(898, 4);
            nn = Int16.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            sn = Int32.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);

            return (new CloudCoin(nn, sn, an, ed, aoid));
        }

        private CoinStack ReadJson(Stream jsonFS)
        {
            jsonFS.Position = 0;
            StreamReader sr = new StreamReader(jsonFS);
            CoinStack stack = null;
            try
            {
                stack = JsonConvert.DeserializeObject<CoinStack>(sr.ReadToEnd());
            }
            catch (JsonException ex)
            {
                throw;
            }
            return stack;
        }
    }
}
