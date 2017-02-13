using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SelectOutStackWindow.xaml
    /// </summary>
    public partial class SelectOutStackWindow : Window
    {
        public SelectOutStackWindow()
        {
            InitializeComponent();
            stacksToSelect.KeyDown += onKeyDown;
            Closing += onWinClose;
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
            if (stacksToSelect.SelectedItem == null)
                MessageBox.Show(this, "Please select desired stack and press OK","Select Stack");
            else
                Hide();
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                onClick(this, e);
            }
        }

        private void onWinClose(object sender, CancelEventArgs e)
        {
            if (stacksToSelect.SelectedItem == null)
                MessageBox.Show(this, "Please select desired stack and press OK", "Select Stack");
            else
                Hide();
        }
    }
}
