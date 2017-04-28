/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using System.Windows;
using System.Windows.Input;

//TODO: Make SelectOutStackViewModel class

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for SelectOutStackWindow.xaml
    /// </summary>
    public partial class SelectOutStackWindow : Window
    {
        public SelectOutStackWindow()
        {
            InitializeComponent();
            Owner = MainWindow.Instance;
            stacksToSelect.KeyDown += onKeyDown;
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                onClick(this, e);
            }
        }

    }
}
