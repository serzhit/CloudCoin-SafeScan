using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CloudCoin_SafeScan
{
    public class CloudCoin
    {
        public enum Type { json, jpeg, unknown }
        public enum Status { fail, pass, error, unknown }
        public enum Denomination {  Unknown, One, Five, Quarter, Hundred, KiloQuarter}

        public Denomination denomination
        {
            get
            {
                if (serial < 1) return Denomination.Unknown;
                else if (serial < 2097153) return Denomination.One;
                else if (serial < 4194305) return Denomination.Five;
                else if (serial < 6291457) return Denomination.Quarter;
                else if (serial < 14680065) return Denomination.Hundred;
                else if (serial < 16777217) return Denomination.KiloQuarter;
                else return Denomination.Unknown;
            }
        }
        public int serial { set; get; }
        public int netnumber { set; get; }
        public string[] ans = new string[25];
        public string[] pans = new string[25];
        public Status[] lastCheckStatus = new Status[25];
        public string[] aoidHex = new string[1];//Account or Owner ID
        public string filename;
        public Type filetype;
        public string expiredHexOn; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018

        // Constructor from args
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            serial = sn;
            netnumber = nn;
            ans.CopyTo(this.ans, 0);
            expiredHexOn = expired;
            aoidHex = aoid;
            filetype = Type.unknown;
            filename = null;
            pans = generatePans();
            for(int i =0; i<RAIDA.NODEQNTY; i++) lastCheckStatus[i] = Status.unknown;
        }

        //Constructor from file with Coin
        public CloudCoin(string filename)
        {
            FileStream ccfile = null;
            try
            {
                ccfile = File.OpenRead(filename);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("File " + filename + " was not found!\n" + e.Message );
            }
            catch (IOException e)
            {
                MessageBox.Show("IO error catched: " + e.Message);
                throw;
            }

            byte[] signature = new byte[3];
            ccfile.Read(signature, 0, 3);

            if (Enumerable.SequenceEqual(signature, new byte[] { 255, 216, 255 }))
            {
                filetype = Type.jpeg;
                ReadFromJpeg(ccfile);

            }
            else if (Enumerable.SequenceEqual(signature, new byte[] { 255, 254, 120 }))
            {
                filetype = Type.json;
                
                
            }
            else filetype = Type.unknown;

            ccfile.Close();
        }

        public void ReadFromJpeg(FileStream fs)
        {
            byte[] fileByteContent = new byte[455];
            string jpegHexContent = "";
            
            fs.Read(fileByteContent, 0, fileByteContent.Length);
            for (int i=0; i< fileByteContent.Length; i++)
            {
                jpegHexContent += fileByteContent[i].ToString("X2");
            }
            
            for(int i=0; i < RAIDA.NODEQNTY; i++)
            {
                ans[i] = jpegHexContent.Substring(40 + i * 32, 32);
            }
            aoidHex[0] = jpegHexContent.Substring(840, 55);
            expiredHexOn = jpegHexContent.Substring(898, 4);
            netnumber = Int16.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier );
            serial = Int32.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);
        }
           
        public string[] generatePans()
        {
            string[] result = new string[RAIDA.NODEQNTY];
            Random rnd = new Random();
            byte[] buf = new byte[16];
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                string aaa = "";
                rnd.NextBytes(buf);
                for (int j = 0; j < buf.Length; j++)
                {
                    aaa += buf[j].ToString("X2");
                }
                result[i] = aaa;
            }
            return result;
        }
    }
}
