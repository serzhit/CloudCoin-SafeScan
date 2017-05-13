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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SetPasswordWindow : Window
    {
        public SetPasswordWindow()
        {
            InitializeComponent();
            Password.KeyDown += onKeyDown;
        }

        private void okButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Count() < 5)
                MessageBox.Show(Properties.Resources.TooShortPassword);
            else if (Password.Password != PasswordVerify.Password)
                MessageBox.Show(Properties.Resources.NotMatchPassword);
            DialogResult = true;
            Close();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
//            Hide();
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                okButtonClicked(this, e);
            }
        }
    }
}
