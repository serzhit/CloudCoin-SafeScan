using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Text.RegularExpressions;
using RestSharp;
using Newtonsoft.Json;
using CloudCoin_SafeScan.Properties;


namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance
        {
            get
            {
                if (theOnlyInstance == null)
                    theOnlyInstance = new MainWindow();
                return theOnlyInstance;
            }

        }
//        RAIDA raida = RAIDA.Instance;

        private static MainWindow theOnlyInstance;

        private MainWindow()
        {
            InitializeComponent();
        }

        private void ImageCheck_Selected(object sender, InputEventArgs e)
        {
            CloudCoinFile coinFile;
            try
            {
                coinFile = new CloudCoinFile(Utils.ChooseInputFile());
                RAIDA.Instance.Detect(coinFile.Coins);
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show("File not found: " + fnfex.Message);
            }
            catch (JsonException jsonex)
            {
                MessageBox.Show("Error in json file format: " + jsonex.Message);
            }
        }

        public void AllEchoesCompleted()
        {
            Dispatcher.Invoke(() =>
            {
                WorkdMapTextBox.Text = "Done. Hover for status";
            });
        }

        public void ShowEchoProgress(RAIDA.EchoResponse result, RAIDA.Node node)
        {
            Dispatcher.Invoke(() =>
            {
                node.LastEchoStatus = result; //recording Echo status to instance
                
                Brush color;
                var tt = new ToolTip();
                tt.Content = node.ToString();
                if (node.LastEchoStatus.status == "ready")
                {
                    color = Brushes.LightGreen;
                    switch (node.Country)
                    {
                        case RAIDA.Countries.Australia:
                            AUstraliaRadioButton.Background = color;
                            AUstraliaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Macedonia:
                            MacedoniaRadioButton.Background = color;
                            MacedoniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Philippines:
                            PhilippinesRadioButton.Background = color;
                            PhilippinesRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Serbia:
                            SerbiaRadioButton.Background = color;
                            SerbiaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Bulgaria:
                            BulgariaRadioButton.Background = color;
                            BulgariaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Sweden:
                            SwedenRadioButton.Background = color;
                            SwedenRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.California:
                            CaliforniaRadioButton.Background = color;
                            CaliforniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.UK:
                            UKRadioButton.Background = color;
                            UKRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Punjab:
                            PenjabRadioButton.Background = color;
                            PenjabRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Banglore:
                            BangloreRadioButton.Background = color;
                            BangloreRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Texas:
                            TexasRadioButton.Background = color;
                            TexasRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.USA1:
                            USA1RadioButton.Background = color;
                            USA1RadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Romania:
                            RomaniaRadioButton.Background = color;
                            RomaniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Taiwan:
                            TaiwanRadioButton.Background = color;
                            TaiwanRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Russia1:
                            MoscowRadioButton.Background = color;
                            MoscowRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Russia2:
                            StPetersburgRadioButton.Background = color;
                            StPetersburgRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Columbia:
                            ColumbiaRadioButton.Background = color;
                            ColumbiaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Singapore:
                            SingaporeRadioButton.Background = color;
                            SingaporeRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Germany:
                            GermanyRadioButton.Background = color;
                            GermanyRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Canada:
                            CanadaRadioButton.Background = color;
                            CanadaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Venezuela:
                            VenezuelaRadioButton.Background = color;
                            VenezuelaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Hyperbad:
                            HyperbadRadioButton.Background = color;
                            HyperbadRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.USA2:
                            USA3RadioButton.Background = color;
                            USA3RadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Switzerland:
                            SwitzerlandRadioButton.Background = color;
                            SwitzerlandRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Luxenburg:
                            LuxemburgRadioButton.Background = color;
                            LuxemburgRadioButton.ToolTip = tt;
                            break;
                    }
                } else
                {
                    color = Brushes.DarkRed;
                    switch (node.Country)
                    {
                        case RAIDA.Countries.Australia:
                            AUstraliaRadioButton.Background = color;
                            AUstraliaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Macedonia:
                            MacedoniaRadioButton.Background = color;
                            MacedoniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Philippines:
                            PhilippinesRadioButton.Background = color;
                            PhilippinesRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Serbia:
                            SerbiaRadioButton.Background = color;
                            SerbiaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Bulgaria:
                            BulgariaRadioButton.Background = color;
                            BulgariaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Sweden:
                            SwedenRadioButton.Background = color;
                            SwedenRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.California:
                            CaliforniaRadioButton.Background = color;
                            CaliforniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.UK:
                            UKRadioButton.Background = color;
                            UKRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Punjab:
                            PenjabRadioButton.Background = color;
                            PenjabRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Banglore:
                            BangloreRadioButton.Background = color;
                            BangloreRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Texas:
                            TexasRadioButton.Background = color;
                            TexasRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.USA1:
                            USA1RadioButton.Background = color;
                            USA1RadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Romania:
                            RomaniaRadioButton.Background = color;
                            RomaniaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Taiwan:
                            TaiwanRadioButton.Background = color;
                            TaiwanRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Russia1:
                            MoscowRadioButton.Background = color;
                            MoscowRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Russia2:
                            StPetersburgRadioButton.Background = color;
                            StPetersburgRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Columbia:
                            ColumbiaRadioButton.Background = color;
                            ColumbiaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Singapore:
                            SingaporeRadioButton.Background = color;
                            SingaporeRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Germany:
                            GermanyRadioButton.Background = color;
                            GermanyRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Canada:
                            CanadaRadioButton.Background = color;
                            CanadaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Venezuela:
                            VenezuelaRadioButton.Background = color;
                            VenezuelaRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Hyperbad:
                            HyperbadRadioButton.Background = color;
                            HyperbadRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.USA2:
                            USA3RadioButton.Background = color;
                            USA3RadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Switzerland:
                            SwitzerlandRadioButton.Background = color;
                            SwitzerlandRadioButton.ToolTip = tt;
                            break;
                        case RAIDA.Countries.Luxenburg:
                            LuxemburgRadioButton.Background = color;
                            LuxemburgRadioButton.ToolTip = tt;
                            break;
                    }
                }
            });
        }



        private void ImageSafe_Selected(object sender, InputEventArgs e)
        {
            Safe.Instance.Show();
        }

        private void ImagePay_Selected(object sender, InputEventArgs e)
        {
            Safe.Instance.SaveOutStack();
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
