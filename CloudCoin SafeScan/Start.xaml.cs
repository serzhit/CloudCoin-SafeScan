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
using Microsoft.Win32;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
        }

        private void ImageCheck_Selected(object sender, InputEventArgs e)
        {
            OpenFileDialog FD = new OpenFileDialog();

            FD.ShowDialog();

            CloudCoin coin = new CloudCoin(FD.FileName);

            CheckCoinsPage checkCoinsPage = new CheckCoinsPage();
            this.NavigationService.Navigate(checkCoinsPage);

//            Stream FileStream = FD.OpenFile();
//Read operations here
//            FileStream.Close();

            MessageBox.Show("You chose " + FD.FileName);
        }

        private void ImageSafe_Selected(object sender, InputEventArgs e)
        {

        }

        private void ImagePay_Selected(object sender, InputEventArgs e)
        {

        }

    }
}
