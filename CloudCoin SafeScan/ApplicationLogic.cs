﻿using System;
using System.IO;
using System.Windows;

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
                coinFile = new CloudCoinFile(FileSystem.ChooseInputFile());
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show("File not found: " + fnfex.Message);
            }

            if(coinFile != null)
            {
                CoinScaner scaner = new CoinScaner(coinFile.Coins);
                MessageBoxResult mbres = MessageBox.Show("Would you like to change ownership and import money in Safe?\nChoosing \"No\" will simply scan coins without changing passwords.", "Change Ownership?", MessageBoxButton.YesNo);
                if(mbres == MessageBoxResult.Yes)
                {
                    scaner.Import();
                }
                else
                {
                    scaner.Scan();
                }
            }
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
                safe.SaveOutStack();
            }
        }
    }
}