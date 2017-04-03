using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public class CoinScaner
    {
        CoinStack ScannedStack;

        public CoinScaner(CoinStack stack)
        {
            ScannedStack = stack;
        }

        public void Scan()
        {
            CheckCoinsWindow checkWin = new CheckCoinsWindow(ScannedStack);
            checkWin.Show();

            RAIDA.Instance.Detect(ScannedStack);
            //            foreach(CloudCoin coin in ScannedStack)
            //            {
            //               coin.pans = coin.an;
            //                RAIDA.Instance.Detect(coin);
            //            }
        }

        public void Import()
        {
            CheckCoinsWindow checkWin = new CheckCoinsWindow(ScannedStack);
            checkWin.Show();

            foreach (CloudCoin coin in ScannedStack)
            {
                RAIDA.Instance.Detect(coin);
            }
        }
    }
}
