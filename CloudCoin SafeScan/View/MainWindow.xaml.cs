/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using System.Windows;

namespace CloudCoin_SafeScan
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance
        {
            get
            {
                if (theOnlyInstance == null)
                {
                    theOnlyInstance = new MainWindow();
                }
                return theOnlyInstance;
            }

        }

        private static MainWindow theOnlyInstance;

        private MainWindow()
        {
            InitializeComponent();
        }
    }
}
