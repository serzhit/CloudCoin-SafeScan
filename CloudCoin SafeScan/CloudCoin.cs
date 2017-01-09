using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CloudCoin_SafeScan
{
    public class CloudCoin
    {
        public enum Type { json, jpeg, unknown }
        public enum Status { fail, pass, error, unknown }

        public short denomination { set; get; }
        public int serial { set; get; }
        public int netnumber { set; get; }
        public string[] ans = new string[25];
        public string[] pans = new string[25];
        public Status[] lastCheckStatus = new Status[25];
        public string[] aoid = new string[1];//Account or Owner ID
        public string filename;
        public Type filetype;
        public string expiredOn; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018

        // Constructor from args
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            serial = sn;
            netnumber = nn;
            this.ans = ans;
            expiredOn = expired;
            this.aoid = aoid;
            filetype = Type.unknown;
            pans = generatePans();
            for(int i =0; i<RAIDA.NODEQNTY; i++) lastCheckStatus[i] = Status.unknown;
        }

        //Constructor from file with Coin
        public CloudCoin(string filename)
        {
            FileStream ccfile = null;
            try
            {
                ccfile = File.Open(filename, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File " + filename + " was not found!");
            }
            catch (IOException e)
            {
                Console.WriteLine("IO error catched. Something wrong with file " + filename + "!");
                throw;
            }

            byte[] signature = new byte[3];
            ccfile.Read(signature, 0, 3);

            if (Enumerable.SequenceEqual(signature, new byte[] { 255, 216, 255 })) {
                filetype = Type.jpeg;
            } else if (Enumerable.SequenceEqual(signature, new byte[] { 255,254,120})) {
                filetype = Type.json;
            }

            
            Console.WriteLine("Unknown file format");
                    

            ccfile.Close();
        }
           
        public string[] generatePans()
        {
            string[] result = { };
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
