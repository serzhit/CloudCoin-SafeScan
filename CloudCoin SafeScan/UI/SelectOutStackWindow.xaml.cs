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
            stacksToSelect.KeyDown += onKeyDown;
        }

        public class Stack4Display
        {
            public int Ones { get; set; }
            public int Fives { get; set; }
            public int Quarters { get; set; }
            public int Hundreds { get; set; }
            public int KiloQuarters { get; set; }
            public int Total { get; set; }
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
