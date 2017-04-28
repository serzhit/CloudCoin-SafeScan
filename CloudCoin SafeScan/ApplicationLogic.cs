/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */

using System;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    internal static class ApplicationLogic
    {
        internal static void MainRun()
        {
            FileSystem.InitializePaths();
            MainWindow.Instance.Show();
            RAIDA.Instance.getEcho();
        }

        internal static void ScanSelected()
        {
            CloudCoinFile coinFile = null;
            try
            {
                string[] files = FileSystem.ChooseInputFile();
                if (files != null)
                {
                    coinFile = new CloudCoinFile(files);
                }
                else
                    return;
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show(Properties.Resources.FnF + fnfex.Message);
            }

            if(coinFile != null && coinFile.IsValidFile)
            {
                try
                {
                    MessageBoxResult mbres = MessageBox.Show(Properties.Resources.CheckOrImport, Properties.Resources.ChangeOwnership, MessageBoxButton.YesNo);
                    if (mbres == MessageBoxResult.Yes)
                    {
                        CheckCoinsWindow checkWin = new CheckCoinsWindow(coinFile.Coins);
                        RAIDA.Instance.Detect(coinFile.Coins, true);
                        checkWin.ShowDialog();

                        Safe.Instance?.Add(coinFile.Coins);
                        checkWin.Close();
                        Safe.Instance?.Show();
                    }
                    else
                    {
                        CheckCoinsWindow checkWin = new CheckCoinsWindow(coinFile.Coins);
                        RAIDA.Instance.Detect(coinFile.Coins, false);
                        checkWin.ShowDialog();
                        checkWin.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show(MainWindow.Instance, ex.Message);
                }
            }
            else
            {
                MessageBox.Show(MainWindow.Instance, coinFile.Filename + "\n: does not contain CloudCoins!");
            }
        }

        internal static void DetectFracked()
        {
            CoinStack fracked = new CoinStack(Safe.Instance.FrackedCoinsList);
            CheckCoinsWindow checkWin = new CheckCoinsWindow(fracked);
            RAIDA.Instance.Detect(fracked, false);
            checkWin.ShowDialog();
            Safe.Instance.Save();


            checkWin.Close();
        }

        internal static void SafeSelected()
        {
            Safe safe;
            try { safe = Safe.Instance; }
            catch (Exception ex)
            {
                safe = null;
            }
            if (safe != null)
            {
                safe.Show();
            }
        }
        internal static void PaySelected()
        {
            Safe safe;

            try { safe = Safe.Instance; }
            catch (TypeInitializationException ex)
            {
                safe = null;
            }

            if (safe != null)
            {
                var howMuch = new WithdrawDialog();
                howMuch.enterSumBox.Focus();
                howMuch.Owner = MainWindow.Instance;
                howMuch.ShowDialog();

                if (howMuch.DialogResult == true)
                {
                    bool isJSON = (bool)howMuch.FormatJSON.IsChecked ? true : false;
                    bool isFull = (bool)howMuch.FullCoins.IsChecked ? true : false;
                    bool isExported = false;
                    string note = howMuch.ExportNote.Text;
                    int desiredSum = int.Parse(howMuch.enterSumBox.Text);

                    if(isFull)
                    {
                        isExported = safe.SaveOutStack(desiredSum, isJSON, note);
                    }
                    else
                    {

                    }
                    
                    if (isExported)
                    {
                        if (isJSON)
                            MessageBox.Show(Properties.Resources.StackExported);
                        else
                            MessageBox.Show(Properties.Resources.JpegExported);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.NothingExported);
                    }
                }
            }
        }

        internal static void FixSelected()
        {
            FixProcessWindow fpw = new FixProcessWindow();
            var task = Task.Run(() =>
            {
                int i = 0;
                foreach (CloudCoin coin in Safe.Instance.FrackedCoinsList)
                {
                    int index = i;
                    RAIDA.Instance.fixCoin(coin, index);
                    i++;
                }
            });
            task.ContinueWith((anc) =>
            {
                Safe.Instance.onSafeContentChanged(new EventArgs());
                Safe.Instance.Save();
            });

            fpw.ShowDialog();
            
        }
    }
}
