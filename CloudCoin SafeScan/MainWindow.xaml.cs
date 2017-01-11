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
            OpenFileDialog FD = new OpenFileDialog();

            FD.ShowDialog();

            CloudCoin coin = new CloudCoin(FD.FileName);

            checkCoinsPage.Show();
            //            this.Hide();

            MessageBox.Show("You chose " + FD.FileName);

            BackgroundWorker[] workerList = new BackgroundWorker[25];
            for (int i = 0; i < 25; i++)
            {
                workerList[i] = new BackgroundWorker();
                workerList[i].DoWork += worker_DoWork;
                workerList[i].RunWorkerCompleted += worker_RunWorkerCompleted;
                workerList[i].RunWorkerAsync(i);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e )
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var client = new RestClient();
            client.BaseUrl = new Uri("https://RAIDA" + e.Argument + ".cloudcoin.global/service");
            var request = new RestRequest("echo");

            try
            {
                RAIDA.Echo r = new RAIDA.Echo();
                e.Result = JsonConvert.DeserializeAnonymousType<RAIDA.Echo>(client.Execute(request).Content, r);
                
            }
            catch (JsonException)
            {
                e.Result = "Invalid response";
                
            }
            
            
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.GetType().Equals(typeof(string))) {
                string result = e.Result as string;
                checkCoinsPage.RAIDA_Check_Log.Text += "Server responded " + result + "\n";
                
            }
            else {
                RAIDA.Echo result = e.Result as RAIDA.Echo;
                checkCoinsPage.RAIDA_Check_Log.Text += "Server  " + result.server + " responded " + result.status + "\n";
            }

            checkCoinsPage.CheckProgress.Value += 4;
            
        }

        private void ImageSafe_Selected(object sender, InputEventArgs e)
        {

        }

        private void ImagePay_Selected(object sender, InputEventArgs e)
        {

        }

    }
}
