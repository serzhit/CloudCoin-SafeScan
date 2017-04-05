using System;
using System.Windows;

//TODO: Make SafeContentViewModel class

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for SafeContents.xaml
    /// </summary>
    public partial class SafeContentWindow : Window
    {
        public SafeContentWindow()
        {
            InitializeComponent();
            Owner = MainWindow.Instance;
        }

        private void onCloseButton(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
