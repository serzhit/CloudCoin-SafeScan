using System;
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
    /// Interaction logic for SafeContents.xaml
    /// </summary>
    public partial class SafeContentWindow : Window
    {
        public class Shelf4Display
        {
            public string Value { get; set; }
            public int Good { get; set; }
            public int Fractioned { get; set; }
            public int Counterfeited { get; set; }
            public int Total { get; set; }
        }
        public SafeContentWindow()
        {
            InitializeComponent();
        }

        private void onCloseButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void onFixButton(object sender, RoutedEventArgs e)
        {
            Safe.Instance.TryFix();
        }
    }
}
