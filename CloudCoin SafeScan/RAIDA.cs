using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    class RAIDA
    {
        public const short NODEQNTY = 25;

        public class Echo
        {
            public string server { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string time { get; set; }

            public Echo()
            {
                server = "unknown";
                status = "unknown";
                message = "empty";
                time = "";
            }
        }
    }
}
