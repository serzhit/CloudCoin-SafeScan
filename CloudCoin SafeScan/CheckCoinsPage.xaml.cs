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

        public int coinsToDetect;
        public double PercentDetected;
        public double ProgressBar
        {
            get
            {
                return CheckProgress.Value;
            }
            set
            {
                CheckProgress.Minimum = 0;
                CheckProgress.Maximum = coinsToDetect * 100;
                if (value >=0 && value <= coinsToDetect * 100)
                    CheckProgress.Value = value;
            }
        }

        public struct Coin4Display
        {
            public string serial;
            public string value;
            public bool auth;
            public string comment;
        }

        public void ShowDetectProgress(RAIDA.DetectResponse result, RAIDA.Node node, CloudCoin coin)
        {
            Dispatcher.Invoke(() =>
            {
                if (result.status == "pass")
                    coin.detectStatus[node.Number] = CloudCoin.raidaNodeResponse.pass;
                else if (result.status == "fail")
                    coin.detectStatus[node.Number] = CloudCoin.raidaNodeResponse.fail;
                else
                    coin.detectStatus[node.Number] = CloudCoin.raidaNodeResponse.error;
                node.LastDetectResult = result;
                RAIDA_Check_Log.Text += "RAIDA" + node.Number + " check " + result.status + ".\n";
                ProgressBar += 4;
                PercentDetected += (double)4/(double)coinsToDetect;
                percentBox.Text = PercentDetected.ToString("F2") + "%";
                

                Brush color;
                var tt = new ToolTip();
                tt.Content = node.ToString(result);
                if (result.status == "pass")
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
                }
                else
                {
                    color = Brushes.OrangeRed;
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

        public void AllDetectCompleted(CoinStack stack)
        {
            Dispatcher.Invoke(() =>
            {
                List<Coin4Display> items = new List<Coin4Display>();
                AuthResultsGrid.ItemsSource = items;

                for (int i=0;i<stack.cloudcoin.Count();i++)
                {
                    var coin = stack.cloudcoin[i];
                    Coin4Display s;
                    s.serial = coin.sn.ToString();
                    s.value = Convert.Denomination2Int(coin.denomination).ToString();
                    s.auth = coin.isPassed;
                    s.comment = coin.percentOfRAIDAPass.ToString() + "% of RAIDA servers uthenticated this coin";
                    items.Add(s);
                }
                
                checkWMapTextBox.Text = "Hover for auth status";
            });
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
