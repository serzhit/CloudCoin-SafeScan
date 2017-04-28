/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
                MessageBox.Show(Properties.Resources.LessThen5Password);
            DialogResult = true;
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
