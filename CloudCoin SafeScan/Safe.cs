using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CloudCoin_SafeScan
{
    public class Safe
    {
        public string password;
        public string safeFilePath;
        public FileInfo safeFileInfo;
        
        public Safe()
        {
            safeFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                "\\Cloudcoin\\" + Environment.UserName + ".safe";
            safeFileInfo = new FileInfo(safeFilePath);
            if(!safeFileInfo.Exists)
            {
                MessageBox.Show("File " + safeFilePath + " does not exist!");
            }
             
        }

        public void save(CoinStack stack)
        {
            //To be implemented
        }
    }
}
