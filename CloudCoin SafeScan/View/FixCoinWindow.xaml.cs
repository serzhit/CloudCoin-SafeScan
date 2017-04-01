using System.Windows;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for FixCoinWindow.xaml
    /// </summary>
    public partial class FixCoinWindow : Window
    {
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
            coinImage.Source = coin.coinImage;
            serialNumber.Content = "S/N: " + coin.sn.ToString();
        }

        public void SetStatusProperty(string value)
        {
            Dispatcher.Invoke(() => 
            {
//                ViewModel.StatusText = value;
                //fixingLbl.Text = value;
            });
        }
        public void DisplayFixResult(ObservableStatus result, int i)
        {
            Dispatcher.Invoke(() =>
            {
//                ViewModel.nodeStatus[i] = result;
            });
        }
     }
}
