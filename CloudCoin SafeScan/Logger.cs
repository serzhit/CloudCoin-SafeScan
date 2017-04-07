using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    public static class Logger
    {
        public static void Initialize()
        {
            string logdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinLogDir);
            var DI = new FileInfo(logdir);
            if (!DI.Exists)
            {
                DI.Create();
            }

            FileInfo LogFile = 
        }
    }
}
