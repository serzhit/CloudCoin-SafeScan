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
            Safe.Instance?.onBeforeFixStart(EventArgs.Empty);
        }
    }
}
