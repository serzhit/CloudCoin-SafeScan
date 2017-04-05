using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    public delegate void EchoStatusChangedEventHandler(object o, EchoStatusChangedEventArgs e);
    public delegate void DetectCoinCompletedEventHandler(object o, DetectCoinCompletedEventArgs e);
    public delegate void StackScanCompletedEventHandler(object o, StackScanCompletedEventArgs e);
    public delegate void SafeContentChangedEventHandler(object o, EventArgs e);
    public delegate void CoinFixStartedEventHandler(object o, CoinFixStartedEventArgs e);
    public delegate void CoinFixFinishedEventHandler(object o, CoinFixFinishedEventArgs e);

    public class EchoStatusChangedEventArgs : EventArgs
    {
        public int index;

        public EchoStatusChangedEventArgs(int index)
        {
            this.index = index;
        }
    }

    public class DetectCoinCompletedEventArgs : EventArgs
    {
        public CloudCoin coin;
        public Stopwatch sw;

        public DetectCoinCompletedEventArgs(CloudCoin cc, Stopwatch stwtch)
        {
            coin = cc;
            sw = stwtch;
        }
    }

    public class StackScanCompletedEventArgs : EventArgs
    {
        public CoinStack stack;
        public Stopwatch sw;

        public StackScanCompletedEventArgs(CoinStack st, Stopwatch stwtch)
        {
            stack = st;
            sw = stwtch;
        }
    }
    public class CoinFixStartedEventArgs : EventArgs
    {
        public int coinindex;
        public int NodeNumber;

        public CoinFixStartedEventArgs(int ci, int nn)
        {
            coinindex = ci;
            NodeNumber = nn;
        }
    }

    public class CoinFixFinishedEventArgs : EventArgs
    {
        public int coinindex;
        public int NodeNumber;
        public CloudCoin.raidaNodeResponse result;

        public CoinFixFinishedEventArgs(int ci, int nn, CloudCoin.raidaNodeResponse res)
        {
            coinindex = ci;
            NodeNumber = nn;
            result = res;
        }
    }


}
