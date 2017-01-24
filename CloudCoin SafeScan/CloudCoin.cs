using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public class CloudCoin
    {
        public enum Type { json, jpeg, unknown }
        public enum Denomination { Unknown, One, Five, Quarter, Hundred, KiloQuarter }
        public enum Status { Authenticated, Counterfeit, Fractioned, Unknown }
        public enum raidaNodeResponse { pass, fail, error, unknown }

        public Denomination denomination
        {
            get
            {
                if (sn < 1) return Denomination.Unknown;
                else if (sn < 2097153) return Denomination.One;
                else if (sn < 4194305) return Denomination.Five;
                else if (sn < 6291457) return Denomination.Quarter;
                else if (sn < 14680065) return Denomination.Hundred;
                else if (sn < 16777217) return Denomination.KiloQuarter;
                else return Denomination.Unknown;
            }
        }
        public ImageSource coinImage
        {
            get
            {
                switch (denomination)
                {
                    case Denomination.One:
                        return new BitmapImage(new Uri(@"Resources/1coin.png", UriKind.Relative));
                    case Denomination.Five:
                        return new BitmapImage(new Uri(@"Resources/5coin.png", UriKind.Relative));
                    case Denomination.Quarter:
                        return new BitmapImage(new Uri(@"Resources/25coin.png", UriKind.Relative));
                    case Denomination.Hundred:
                        return new BitmapImage(new Uri(@"Resources/100coin.png", UriKind.Relative));
                    case Denomination.KiloQuarter:
                        return new BitmapImage(new Uri(@"Resources/250coin.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri(@"Resources/stackcoins.png", UriKind.Relative));
                }
            }
        }
        public int sn { set; get; }
        public int nn { set; get; }
        public string[] an = new string[RAIDA.NODEQNTY];
        public string[] pans = new string[RAIDA.NODEQNTY];
        public raidaNodeResponse[] detectStatus = new raidaNodeResponse[RAIDA.NODEQNTY];
        public string[] aoid = new string[1];//Account or Owner ID
        public string filename;
        public Type filetype;
        public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018
        public int percentOfRAIDAPass
        {
            get
            {
                return detectStatus.Count(element => element == raidaNodeResponse.pass) / detectStatus.Count();
            }
        }

        CloudCoin.Status status
        {
            get
            {
                if (percentOfRAIDAPass != 1)
                    return isPassed ? CloudCoin.Status.Fractioned : CloudCoin.Status.Counterfeit;
                else
                    return isPassed ? CloudCoin.Status.Authenticated : CloudCoin.Status.Counterfeit;
            }
        }

        bool isPassed
        {
            get
            {
                return (detectStatus.Count(element => element == raidaNodeResponse.pass) > 3) ? true : false;
            }
        }

        // Constructor from args
        [JsonConstructor]
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            this.sn = sn;
            this.nn = nn;
            ans = an;
            ed = expired;
            this.aoid = aoid;
            filetype = Type.json;
            filename = null;
            pans = generatePans();
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = raidaNodeResponse.unknown;
        }

        //Constructor from file with Coin
        public CloudCoin(FileStream jpegFS)
        {
            filetype = Type.jpeg;
            byte[] fileByteContent = new byte[455];
            int numBytesToRead = fileByteContent.Length;
            int numBytesRead = 0;
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
            jpegHexContent = Convert.ToHexString(fileByteContent);

            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                an[i] = jpegHexContent.Substring(40 + i * 32, 32);
            }
            aoid[0] = jpegHexContent.Substring(840, 55);
            ed = jpegHexContent.Substring(898, 4);
            nn = Int16.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            sn = Int32.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);

            pans = generatePans();
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = raidaNodeResponse.unknown;
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

    [JsonObject]
    public class CoinStack : IEnumerable<CloudCoin>
    {
        public IList<CloudCoin> cloudcoin { get; set; }
        public int coinsInStack
        {
            get
            {
                int s = 0;
                foreach (CloudCoin coin in cloudcoin)
                {
                    s++;
                }
                return s;
            }
        }
        public int SumInStack
        {
            get
            {
                int s = 0;
                foreach (CloudCoin coin in cloudcoin)
                {
                    s += Convert.Denomination2Int(coin.denomination);
                }
                return s;
            }
        }

        public CoinStack()
        {
            cloudcoin = new List<CloudCoin>();
        }

        public CoinStack(CloudCoin coin)
        {
            CloudCoin[] _collection = { coin };
            cloudcoin = new List<CloudCoin>(_collection);
//            cloudcoin[0] = coin;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<CloudCoin> GetEnumerator()
        {
            return cloudcoin.GetEnumerator();
        }
    }
}
