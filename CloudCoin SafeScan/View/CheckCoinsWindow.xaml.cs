using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;


//TODO - Make CheckCoinsViewModel class

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for CheckCoinsWindow.xaml
    /// </summary>
    public partial class CheckCoinsWindow : Window
    {
        public CheckCoinsWindow()
        {
            InitializeComponent();
        }

        public CheckCoinsWindow(CoinStack stack)
        {
            InitializeComponent();
            CheckCoinsWindowViewModel VM = new CheckCoinsWindowViewModel(stack);
            DataContext = VM;
            RAIDA.Instance.StackScanCompleted += StackScanCompleted;
        }

        private void StackScanCompleted(object o, StackScanCompletedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                CoinImage.Visibility = Visibility.Hidden;
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
