﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        public enum raidaNodeResponse { pass, fail, error, fixing, unknown }

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
        public int nn { set; get; }
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

        [JsonProperty]
        public int sn { set; get; }
        [JsonProperty]
        public string[] an = new string[RAIDA.NODEQNTY];
        public string[] pans = new string[RAIDA.NODEQNTY];
        [JsonProperty]
        public raidaNodeResponse[] detectStatus;
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
                return detectStatus.Count(element => element == raidaNodeResponse.pass) * 100 / detectStatus.Count();
            }
        }

        public bool isPassed
        {
            get
            {
                return (detectStatus.Count(element => element == raidaNodeResponse.pass) > RAIDA.MINTRUSTEDNODES4AUTH) ? true : false;
            }
        }

        public bool isValidated { get; set; }

        // Constructor from args
        [JsonConstructor]
        public CloudCoin(int nn, int sn, string[] ans, string expired, string[] aoid)
        {
            this.sn = sn;
            this.nn = nn;
            an = ans;
            ed = expired;
            this.aoid = aoid;

            detectStatus = new raidaNodeResponse[RAIDA.NODEQNTY];
            for (int i = 0; i < RAIDA.NODEQNTY; i++) detectStatus[i] = raidaNodeResponse.unknown;

            pans = generatePans();
            isValidated = Validate();
        }


        public bool Validate()
        {
            if(nn == 1 && sn>=0 && sn < 16777216
                && aoid != null && an !=null && an.Length > 0 )
            {
                foreach(var anValue in an)
                {
                    if (anValue.Length != 32)
                        return false;
                }
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
            using (var provider = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < RAIDA.NODEQNTY; i++)
                {
                    var bytes = new byte[16];
                    provider.GetBytes(bytes);

                    Guid pan = new Guid(bytes);
                    String rawpan = pan.ToString("N");
                    String fullPan = "";
                    switch (rawpan.Length)//Make sure the pan is 32 characters long. The odds of this happening are slim but it will happen.
                    {
                        case 27: fullPan = ("00000" + rawpan); break;
                        case 28: fullPan = ("0000" + rawpan); break;
                        case 29: fullPan = ("000" + rawpan); break;
                        case 30: fullPan = ("00" + rawpan); break;
                        case 31: fullPan = ("0" + rawpan); break;
                        case 32: fullPan = rawpan; break;
                        case 33: fullPan = rawpan.Substring(0, rawpan.Length - 1); break;//trim one off end
                        case 34: fullPan = rawpan.Substring(0, rawpan.Length - 2); break;//trim one off end
                    }
                    result[i] = fullPan;
                }
            }//end for each Pan
            return result;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CoinStack : IEnumerable<CloudCoin>
    {
        [JsonProperty]
        public HashSet<CloudCoin> cloudcoin { get; set; }
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
                foreach (CloudCoin cccc in cloudcoin)
                {
                    s += Utils.Denomination2Int(cccc.denomination);
                }
                return s;
            }
        }
        public int SumOfGoodCoins
        {
            get
            {
                int s = 0;
                foreach (CloudCoin cccc in cloudcoin)
                {
                    if(cccc.Verdict != CloudCoin.Status.Counterfeit)
                    {
                        s += Utils.Denomination2Int(cccc.denomination);
                    }
                    
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
            cloudcoin = new HashSet<CloudCoin>();
        }
        public CoinStack(CloudCoin coin)
        {
            CloudCoin[] _collection = { coin };
            cloudcoin = new HashSet<CloudCoin>(_collection);
        }

        [JsonConstructor]
        public CoinStack(HashSet<CloudCoin> list)
        {
            cloudcoin = list;
        }

        public CoinStack(IEnumerable<CloudCoin> collection)
        {
            cloudcoin = new HashSet<CloudCoin>(collection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<CloudCoin> GetEnumerator()
        {
            return cloudcoin.GetEnumerator();
        }
        public void Add(CloudCoin coin)
        {
            cloudcoin.Add(coin);
            var tmp = cloudcoin.Distinct();
            cloudcoin = new HashSet<CloudCoin>(tmp);
        }
        public void Add(CoinStack stack2)
        {
            foreach (CloudCoin coin in stack2)
            {
                cloudcoin.Add(coin);
            }
            var tmp = cloudcoin.Distinct();
            cloudcoin = new HashSet<CloudCoin>(tmp);
        }

        public void Remove(CoinStack stack2)
        {
            foreach (CloudCoin coin in stack2)
            {
                cloudcoin.Remove(coin);
            }
        }
    }
}
