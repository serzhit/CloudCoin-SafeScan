using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for FixCoinWindow.xaml
    /// </summary>
    public partial class FixCoinWindow : Window
    {
        public FixCoinWindow()
        {
            InitializeComponent();
        }

        public FixCoinWindow(CloudCoin coin)
        {
            InitializeComponent();
            coinImage.Source = coin.coinImage;
            serialNumber.Content = coin.sn.ToString();
            for(int i=0;i<RAIDA.NODEQNTY;i++)
            {
                switch (i)
                {
                    case 0:
                        if(coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _0.Fill = Brushes.Green;
                        break;
                    case 1:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _1.Fill = Brushes.Green;
                        break;
                    case 2:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _2.Fill = Brushes.Green;
                        break;
                    case 3:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _3.Fill = Brushes.Green;
                        break;
                    case 4:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _4.Fill = Brushes.Green;
                        break;
                    case 5:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _5.Fill = Brushes.Green;
                        break;
                    case 6:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _6.Fill = Brushes.Green;
                        break;
                    case 7:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _7.Fill = Brushes.Green;
                        break;
                    case 8:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _8.Fill = Brushes.Green;
                        break;
                    case 9:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _9.Fill = Brushes.Green;
                        break;
                    case 10:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _10.Fill = Brushes.Green;
                        break;
                    case 11:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _11.Fill = Brushes.Green;
                        break;
                    case 12:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _12.Fill = Brushes.Green;
                        break;
                    case 13:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _13.Fill = Brushes.Green;
                        break;
                    case 14:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _14.Fill = Brushes.Green;
                        break;
                    case 15:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _15.Fill = Brushes.Green;
                        break;
                    case 16:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _16.Fill = Brushes.Green;
                        break;
                    case 17:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _17.Fill = Brushes.Green;
                        break;
                    case 18:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _18.Fill = Brushes.Green;
                        break;
                    case 19:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _19.Fill = Brushes.Green;
                        break;
                    case 20:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _20.Fill = Brushes.Green;
                        break;
                    case 21:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _21.Fill = Brushes.Green;
                        break;
                    case 22:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _22.Fill = Brushes.Green;
                        break;
                    case 23:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _23.Fill = Brushes.Green;
                        break;
                    case 24:
                        if (coin.detectStatus[i] == CloudCoin.raidaNodeResponse.pass)
                            _24.Fill = Brushes.Green;
                        break;
                }
            }
        }

        public void Paint(int i, SolidColorBrush brush)
        {
           switch (i)
            {
                case 0:
                    _0.Fill = brush;
                    break;
                case 1:
                    _1.Fill = brush;
                    break;
                case 2:
                    _2.Fill = brush;
                    break;
                case 3:
                    _3.Fill = brush;
                    break;
                case 4:
                    _4.Fill = brush;
                    break;
                case 5:
                    _5.Fill = brush;
                    break;
                case 6:
                    _6.Fill = brush;
                    break;
                case 7:
                    _7.Fill = brush;
                    break;
                case 8:
                    _8.Fill = brush;
                    break;
                case 9:
                    _9.Fill = brush;
                    break;
                case 10:
                    _10.Fill = brush;
                    break;
                case 11:
                    _11.Fill = brush;
                    break;
                case 12:
                    _12.Fill = brush;
                    break;
                case 13:
                    _13.Fill = brush;
                    break;
                case 14:
                    _14.Fill = brush;
                    break;
                case 15:
                    _15.Fill = brush;
                    break;
                case 16:
                    _16.Fill = brush;
                    break;
                case 17:
                    _17.Fill = brush;
                    break;
                case 18:
                    _18.Fill = brush;
                    break;
                case 19:
                    _19.Fill = brush;
                    break;
                case 20:
                    _20.Fill = brush;
                    break;
                case 21:
                    _21.Fill = brush;
                    break;
                case 22:
                    _22.Fill = brush;
                    break;
                case 23:
                    _23.Fill = brush;
                    break;
                case 24:
                    _24.Fill = brush;
                    break;
            }
        }
    }
}
