using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for HowMuchWindow.xaml
    /// </summary>
    public partial class HowMuchWindow : Window
    {
        public HowMuchWindow()
        {
            InitializeComponent();
            enterSumBox.KeyDown += onKeyDown;
            Closing += onWinClose;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back) //The  character represents a backspace
            {
                e.Handled = false; //Do not reject the input
            }
            else if (e.Key == Key.Return)
            {
                if (enterSumBox.Text != "")
                    onOKClicked(this, e);
            }
            else
            {
                e.Handled = true; //Reject the input
            }
        }

        private void onOKClicked(object sender, RoutedEventArgs e)
        {
            short sum = short.Parse(enterSumBox.Text);
            if (sum <= Safe.Instance?.Contents.SumInStack)
            {
                DialogResult = true;
  //              Hide();
            }
            else
                MessageBox.Show(this, "You don't have such amount in Safe.\nTry another value", "Enter anther value");
        }

        private void onWinClose(object sender, CancelEventArgs e)
        {
//            DialogResult = false;
        }
    }
}
