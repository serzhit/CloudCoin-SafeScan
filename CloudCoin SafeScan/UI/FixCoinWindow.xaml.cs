using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public FixCoinWindowViewModel ViewModel;


        public FixCoinWindow()
        {
//            ViewModel = new FixCoinWindowViewModel(coin);
//            DataContext = ViewModel;
            InitializeComponent();
//            coinImage.Source = coin.coinImage;
//            serialNumber.Content = "S/N: " + coin.sn.ToString();
        }

        public void Load(CloudCoin coin)
        {
            ViewModel = new FixCoinWindowViewModel(coin);
            DataContext = ViewModel;
            coinImage.Source = coin.coinImage;
            serialNumber.Content = "S/N: " + coin.sn.ToString();
        }

        public void SetStatusProperty(string value)
        {
            Dispatcher.Invoke(() => 
            {
                ViewModel.StatusText = value;
                //fixingLbl.Text = value;
            });
        }
        public void DisplayFixResult(bool result, int i)
        {
            Dispatcher.Invoke(() =>
            {
                if (result)
                    ViewModel.nodeStatus[i] = true;
                else
                    ViewModel.nodeStatus[i] = false;
            });
        }
     }
}
