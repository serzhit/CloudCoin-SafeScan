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
    /// Interaction logic for FixProcessWindow.xaml
    /// </summary>
    public partial class FixProcessWindow : Window
    {
        public FixStackViewModel ViewModel;

        public FixProcessWindow()
        {
            InitializeComponent();
        }

        public void Load(CoinStack stack)
        {
            ViewModel = new FixStackViewModel(stack);
            DataContext = ViewModel;
            
        }
        public void Load(List<CoinStack> stack)
        {
            ViewModel = new FixStackViewModel(stack);
            DataContext = ViewModel;

        }
    }
}
