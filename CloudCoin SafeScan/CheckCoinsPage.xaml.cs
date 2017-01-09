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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for CheckCoinsPage.xaml
    /// </summary>
    public partial class CheckCoinsPage : Window
    {
        public CheckCoinsPage()
        {
            InitializeComponent();
        }

        public double ProgressBar
        {
            set
            {
                this.CheckProgress.Minimum = 0;
                this.CheckProgress.Maximum = 100;
                if (value >=0 && value <= 100)
                    this.CheckProgress.Value = value;
            }
        }
 
    }
}
