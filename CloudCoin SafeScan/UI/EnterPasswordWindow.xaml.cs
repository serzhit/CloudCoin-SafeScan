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
    /// Interaction logic for EnterPassword.xaml
    /// </summary>
    public partial class EnterPasswordWindow : Window
    {
        public EnterPasswordWindow()
        {
            InitializeComponent();
            passwordBox.KeyDown += onKeyDown;
        }

        private void onOKClick(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Count() < 5)
                MessageBox.Show("Password cannot be less than 5 characters");
            Hide();
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                onOKClick(this, e);
            }
        }
    }
}
