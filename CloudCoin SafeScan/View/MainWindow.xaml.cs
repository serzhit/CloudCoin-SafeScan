using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;

//TODO: Make MainViewModel class

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
