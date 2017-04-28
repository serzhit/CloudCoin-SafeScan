/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
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
