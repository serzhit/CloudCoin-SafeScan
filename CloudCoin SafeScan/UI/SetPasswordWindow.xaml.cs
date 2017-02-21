using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                MessageBox.Show("Password is too short. Use more than 5 characters");
            else if (Password.Password != PasswordVerify.Password)
                MessageBox.Show("Passwords don't match");
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

        private void onTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void onVerifyTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
