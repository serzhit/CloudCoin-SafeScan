using System;
using System.Windows;
using System.Globalization;
using System.Resources;
using GalaSoft.MvvmLight.Threading;

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherHelper.Initialize(); // Gets UIDispatcher property from main thread

            try
            {
                int winVersion = Environment.OSVersion.Version.Major;
                if (winVersion < 6)
                {
                    MessageBox.Show(CloudCoin_SafeScan.Properties.Resources.LowWinVer);
                    Shutdown();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(CloudCoin_SafeScan.Properties.Resources.ErrWinVer);
                Shutdown();
            }
        }

        private void onAppStartup(object sender, StartupEventArgs e)
        {
            ApplicationLogic.MainRun();
        }

        public void ImportCloudCoinFile()
        {

        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = CloudCoin_SafeScan.Properties.Resources.UnhandledException + e.ExceptionObject;
            MessageBox.Show(message);
        }
    }
}
