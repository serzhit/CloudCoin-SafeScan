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
        CheckCoinsPage checkCoinsPage = new CheckCoinsPage();

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
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = "ВСЁ ПРОПАЛО!!! Произошло не обработанное исключение. Сейчас нужно сделать следущее: \r ";
            message += "1) Сохранить изображение этого окна, нажав PrintScrin на клавиатуре и вставить изображение в Paint  \r ";
            message += "2) Написать подробности произошедшего инцидента. При каких обстоятельствах программа упала  \r ";
            message += "3) Выслать изображение этого окна и описание ситуации на адрес: alexey@o-s-a.net \r ";
            message += "4) Если ситуация повториться, вероятно будет нужно очистить папку Engine и QuikTrades что рядом с роботом  \r ";
            message += "5) Возможно придётся удалить процесс Os.Engine из диспетчера задач руками.  \r ";
            message += "6) Ошибка:  " + e.ExceptionObject;

            MessageBox.Show(message);
        }

        private void ImageCheck_Selected(object sender, InputEventArgs e)
        {
            
            RAIDA raida = new RAIDA();

            OpenFileDialog FD = new OpenFileDialog();
            FD.Title = "Choose file with Cloudcoin(s)";
            FD.ShowDialog();


            CloudCoin coin = new CloudCoin(FD.FileName);
            checkCoinsPage.Filename.Text = coin.filename;

            foreach (RAIDA.Node node in raida.NodesArray)
            {
                Task<RAIDA.EchoResponse> task = Task.Run(() => node.Echo());
                Task cont = task.ContinueWith( delegate { ShowEchoProgress(task.Result); });
            }
            checkCoinsPage.CoinImage.Source = coin.coinImage;
            
            checkCoinsPage.Show();

        }

        private void ShowEchoProgress(RAIDA.EchoResponse result)
        {
            Dispatcher.Invoke(() =>
            {
                checkCoinsPage.RAIDA_Check_Log.Text += "Server " + result.server + " responded " + result.status + "\n";
                checkCoinsPage.CheckProgress.Value += 100 / RAIDA.NODEQNTY;
                if (checkCoinsPage.CheckProgress.Value == 100)
                    checkCoinsPage.LabelStatus.Visibility = Visibility.Visible;
            });
        }

        private void ImageSafe_Selected(object sender, InputEventArgs e)
        {

        }

        private void ImagePay_Selected(object sender, InputEventArgs e)
        {

        }

    }
}
