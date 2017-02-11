﻿using System;
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
    /// Interaction logic for HowMuchWindow.xaml
    /// </summary>
    public partial class HowMuchWindow : Window
    {
        public HowMuchWindow()
        {
            InitializeComponent();
            enterSumBox.KeyDown += onKeyDown;
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
                Close();
            }
            else
            {
                e.Handled = true; //Reject the input
            }
        }

        private void onOKClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
