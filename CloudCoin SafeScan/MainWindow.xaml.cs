using System;
using System.ComponentModel;
using System.Collections.Generic;
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
using Microsoft.Win32;
using RestSharp;
using Newtonsoft.Json;


namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        RAIDA raida = new RAIDA();

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                int winVersion = Environment.OSVersion.Version.Major;
                if (winVersion < 6)
                {
                    MessageBox.Show(
                        "Your Windows version is too low. This app works on Windows7 and higher");
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error trying to check Windows version The program was closed.");
                Close();
            }

            Task<RAIDA.EchoResponse>[] tasks = new Task<RAIDA.EchoResponse>[RAIDA.NODEQNTY];
            int i = 0;
            foreach (RAIDA.Node node in raida.NodesArray)
            {
                tasks[i] = Task.Factory.StartNew(() => node.Echo());
                Task cont = tasks[i].ContinueWith(ancestor => { ShowEchoProgress(ancestor.Result, node); });
                i++;
            }
            Task.Factory.ContinueWhenAll(tasks, delegate { AllEchoesCompleted(); });
            

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = "Unhandled exception catched: " + e.ExceptionObject;

            MessageBox.Show(message);
        }

        private void ImageCheck_Selected(object sender, InputEventArgs e)
        {
            OpenFileDialog FD = new OpenFileDialog();
            FD.Title = "Choose file with Cloudcoin(s)";
            if(FD.ShowDialog() == true)
            {
                try
                {
                    CheckCoinsPage checkCoinsPage = new CheckCoinsPage();
                    using (FileStream fsSource = File.Open(FD.FileName, FileMode.Open))
                    //new FileStream(FD.FileName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] signature = new byte[3];
                        fsSource.Read(signature, 0, 3);

                        if (Enumerable.SequenceEqual(signature, new byte[] { 255, 216, 255 })) //JPEG
                        {
                            fsSource.Position = 0;
                            CloudCoin coin = new CloudCoin(fsSource);
                            CoinStack stack = new CoinStack(coin);
                            checkCoinsPage.Filename.Text = FD.SafeFileName;
                            checkCoinsPage.CoinImage.Source = coin.coinImage;
                            checkCoinsPage.coinsToDetect = 1;
                            checkCoinsPage.Show();

                            Task<RAIDA.DetectResponse>[] tasks = new Task<RAIDA.DetectResponse>[RAIDA.NODEQNTY];
                            int i = 0;
//                            List<RAIDA.DetectResponse> res = new List<RAIDA.DetectResponse>(1);
                            foreach (RAIDA.Node node in raida.NodesArray)
                            {
                                tasks[i] = Task.Factory.StartNew(() => node.Detect(coin));
                                Task cont = tasks[i].ContinueWith(ancestor => { checkCoinsPage.ShowDetectProgress(ancestor.Result, node, coin); });
                                i++;
                            }
                            Task.Factory.ContinueWhenAll(tasks, delegate { checkCoinsPage.AllDetectCompleted(stack); });

                        }
                        else if (Enumerable.SequenceEqual(signature, new byte[] { 123, 32, 34 }))  //JSON
                        {
                            fsSource.Position = 0;
                            StreamReader sr = new StreamReader(fsSource);
                            CoinStack stack = null;

                            try
                            {
                                stack = JsonConvert.DeserializeObject<CoinStack>(sr.ReadToEnd());
                                checkCoinsPage.Filename.Text = FD.SafeFileName;
                                checkCoinsPage.CoinImage.Source = new BitmapImage(new Uri(@"Resources/stackcoins.png", UriKind.Relative));
                                checkCoinsPage.coinsToDetect = stack.coinsInStack;
                                checkCoinsPage.Show();

                                foreach(CloudCoin coin in stack)
                                {
                                    Task<RAIDA.DetectResponse>[] tasks = new Task<RAIDA.DetectResponse>[RAIDA.NODEQNTY];
                                    int i = 0;
                                    foreach (RAIDA.Node node in raida.NodesArray)
                                    {
                                        tasks[i] = Task.Factory.StartNew(() => node.Detect(coin));
                                        Task cont = tasks[i].ContinueWith(ancestor => { checkCoinsPage.ShowDetectProgress(ancestor.Result, node, coin); });
                                        i++;
                                    }
                                    Task.Factory.ContinueWhenAll(tasks, delegate { checkCoinsPage.AllDetectCompleted(stack); });
                                }
                            }
                            catch (Exception jsonex)
                            {
                                MessageBox.Show("Error in file format: " + jsonex.Message);
                            }
                            if (stack != null)
                                MessageBox.Show("You have " + stack.coinsInStack + " coins in stack.\nTotal stack value = " + stack.SumInStack);

                        }
                        else
                            MessageBox.Show("Unknown file format. Try find CloudCoin file.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Error reading " + FD.FileName + " from disk!\n" + ex.Message);
                }
                
            }   
        }

        private void AllEchoesCompleted()
        {
            Dispatcher.Invoke(() =>
            {
                WorkdMapTextBox.Text = "Done. Hover for status";
            });
        }

        private void ShowEchoProgress(RAIDA.EchoResponse result, RAIDA.Node node)
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
                }
            });
        }



        private void ImageSafe_Selected(object sender, InputEventArgs e)
        {

        }

        private void ImagePay_Selected(object sender, InputEventArgs e)
        {

        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
