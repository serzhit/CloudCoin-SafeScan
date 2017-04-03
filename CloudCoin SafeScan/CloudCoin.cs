using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudCoin : ICloudCoin
    {
        public enum Denomination { Unknown, One, Five, Quarter, Hundred, KiloQuarter }
        public enum Status { Authenticated, Counterfeit, Fractioned, Unknown }
        public enum raidaNodeResponse { pass, fail, error, unknown }

        public class CoinComparer : IComparer<CloudCoin>
        {
            int IComparer<CloudCoin>.Compare(CloudCoin coin1, CloudCoin coin2)
            {
                return (coin1.sn.CompareTo(coin2.sn));
            }
        }
        public class CoinEqualityComparer : IEqualityComparer<CloudCoin>
        {
            bool IEqualityComparer<CloudCoin>.Equals(CloudCoin x, CloudCoin y)
            {
                return x.sn == y.sn;
            }

            int IEqualityComparer<CloudCoin>.GetHashCode(CloudCoin obj)
            {
                return obj.sn;
            }
        }

        [JsonProperty]
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
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/1coin.png", UriKind.Absolute));
                    case Denomination.Five:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/5coin.png", UriKind.Absolute));
                    case Denomination.Quarter:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/25coin.png", UriKind.Absolute));
                    case Denomination.Hundred:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/100coin.png", UriKind.Absolute));
                    case Denomination.KiloQuarter:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/250coin.png", UriKind.Absolute));
                    default:
                        return new BitmapImage(new Uri(@"pack://application:,,,/Resources/stackcoins.png", UriKind.Absolute));
                }
            }
        }
        [JsonProperty]
        public int sn { set; get; }
        [JsonProperty]
        public int nn { set; get; }
        [JsonProperty]
        public string[] an = new string[RAIDA.NODEQNTY];
        public string[] pans = new string[RAIDA.NODEQNTY];
        public RAIDA.DetectResponse[] detectStatus;
        [JsonProperty]
        public string[] aoid = new string[1];//Account or Owner ID
        
        [JsonProperty]
        public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018
        public Status Verdict
        {
            get
            {
                if (percentOfRAIDAPass != 100)
                    return isPassed ? Status.Fractioned : Status.Counterfeit;
                else
                    return isPassed ? Status.Authenticated : Status.Counterfeit;
            }
        }

        public int percentOfRAIDAPass
        {
            get
            {
                return detectStatus.Count(element => element.status == "pass" || element.status == "error") * 100 / detectStatus.Count();
            }
        }

        public bool isPassed
        {
            get
            {
                return (detectStatus.Count(element => element.status == "pass") > RAIDA.MINTRUSTEDNODES4AUTH) ? true : false;
            }
        }

        // Constructor from args
        [JsonConstructor]
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            this.sn = sn;
            this.nn = nn;
            an = ans;
            ed = expired;
            this.aoid = aoid;
//            filetype = Type.json;
//            filename = null;
            pans = generatePans();
            detectStatus = new RAIDA.DetectResponse[RAIDA.NODEQNTY];
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = new RAIDA.DetectResponse("unknown",sn.ToString(),"unknown","empty",DateTime.Now.ToString());
        }

        //Constructor from file with Coin
        public CloudCoin(Stream jpegFS)
        {
            // TODO: catch exception for wrong file format
//            filetype = Type.jpeg;
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
            jpegHexContent = Utils.ToHexString(fileByteContent);

            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                an[i] = jpegHexContent.Substring(40 + i * 32, 32);
            }
            aoid[0] = jpegHexContent.Substring(840, 55);
            ed = jpegHexContent.Substring(898, 4);
            nn = Int16.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            sn = Int32.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);

            pans = generatePans();
            detectStatus = new RAIDA.DetectResponse[RAIDA.NODEQNTY];
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = new RAIDA.DetectResponse("unknown", sn.ToString(), "unknown", "empty", DateTime.Now.ToString());
        }

        public bool Calibrate()
        {
            if(nn == 1 && sn>=0 && sn < 16777216)
            {
                return true;
            }
            else
            {
                return false;
            }
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

    [JsonObject(MemberSerialization.OptIn)]
    public class CoinStack : IEnumerable<CloudCoin>
    {
        [JsonProperty]
        public List<CloudCoin> cloudcoin { get; set; }
        public int coinsInStack
        {
            get
            {
                return cloudcoin.Count();
            }
        }
        public int SumInStack
        {
            get
            {
                int s = 0;
                foreach (CloudCoin coin in cloudcoin)
                {
                    s += Utils.Denomination2Int(coin.denomination);
                }
                return s;
            }
        }
        public int Ones
        {
            get
            {
                return cloudcoin.Count(x => x.denomination == CloudCoin.Denomination.One);
            }
        }
        public int Fives
        {
            get
            {
                return cloudcoin.Count(x => x.denomination == CloudCoin.Denomination.Five);
            }
        }
        public int Quarters
        {
            get
            {
                return cloudcoin.Count(x => x.denomination == CloudCoin.Denomination.Quarter);
            }
        }
        public int Hundreds
        {
            get
            {
                return cloudcoin.Count(x => x.denomination == CloudCoin.Denomination.Hundred);
            }
        }
        public int KiloQuarters
        {
            get
            {
                return cloudcoin.Count(x => x.denomination == CloudCoin.Denomination.KiloQuarter);
            }
        }
        public int AuthenticatedQuantity
        {
            get
            {
                return cloudcoin.Count(x => x.Verdict == CloudCoin.Status.Authenticated);
            }
        }
        public int FractionedQuantity
        {
            get
            {
                return cloudcoin.Count(x => x.Verdict == CloudCoin.Status.Fractioned);
            }
        }
        public int CounterfeitedQuantity
        {
            get
            {
                return cloudcoin.Count(x => x.Verdict == CloudCoin.Status.Counterfeit);
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
        public void Add(CoinStack stack2)
        {
            cloudcoin.AddRange(stack2);
            cloudcoin = cloudcoin.Distinct( new CloudCoin.CoinEqualityComparer() ).ToList();
        }
    }
}
