using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    class SafeContentWindowViewModel
    {
            public string Value { get; set; }
            public int Good { get; set; }
            public int Fractioned { get; set; }
            public int Counterfeited { get; set; }
            public int Total { get; set; }
    }
}
