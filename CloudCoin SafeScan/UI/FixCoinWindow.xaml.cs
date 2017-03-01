using System.Windows;

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
