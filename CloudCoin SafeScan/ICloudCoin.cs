using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    internal interface ICloudCoin
    {
        CloudCoin.Denomination denomination { get; }

        int sn { set; get; }
        int nn { set; get; }
        CloudCoin.Status Verdict { get; }
        int percentOfRAIDAPass { get; }
        bool isPassed { get; }

        string[] generatePans(int sn);
    }
}
