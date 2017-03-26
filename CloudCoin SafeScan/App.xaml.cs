using System;
using System.Windows;
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
            //Startup += onAppStartup;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherHelper.Initialize(); // Gets UIDispatcher property from main thread

            try
            {
                int winVersion = Environment.OSVersion.Version.Major;
                if (winVersion < 6)
                {
                    MessageBox.Show(
                        "Your Windows version is too low. This app works on Windows7 and higher");
                    Shutdown();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error trying to check Windows version The program was closed.");
                Shutdown();
            }
        }

        private void onAppStartup(object sender, StartupEventArgs e)
        {
            CloudCoin_SafeScan.MainWindow.Instance.Show();
            RAIDA.Instance.getEcho();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = "Unhandled exception catched: " + e.ExceptionObject;
            MessageBox.Show(message);
        }
    }
}
