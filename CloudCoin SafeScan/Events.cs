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


}
