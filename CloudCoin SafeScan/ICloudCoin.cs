/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */

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

        string[] generatePans();
    }
}
