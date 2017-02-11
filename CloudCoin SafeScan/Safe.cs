using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Security.AccessControl;
using CryptSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public sealed class Safe
    {
        private static Safe theOnlySafeInstance = new Safe();
        public static Safe Instance
        {
            get
            {
                return theOnlySafeInstance;
            }
        }

        public string password = null;
        public string safeFilePath;
        public FileInfo safeFileInfo;
        private string cryptedPass;
        public CoinStack Contents;
        public Shelf Ones
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.One);
            }
        }
        public Shelf Fives
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Five);
            }
        }
        public Shelf Quarters
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Quarter);
            }
        }
        public Shelf Hundreds
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Hundred);
            }
        }
        public Shelf KiloQuarters
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.KiloQuarter);
            }
        }

        public class Shelf
        {
            public Safe current;
            CloudCoin.Denomination denomination;
            public int TotalQuantity
            {
                get
                {
                    switch (denomination)
                    {
                        case CloudCoin.Denomination.One:
                            return current.Contents.Ones;
                        case CloudCoin.Denomination.Five:
                            return current.Contents.Fives;
                        case CloudCoin.Denomination.Quarter:
                            return current.Contents.Quarters;
                        case CloudCoin.Denomination.Hundred:
                            return current.Contents.Hundreds;
                        case CloudCoin.Denomination.KiloQuarter:
                            return current.Contents.KiloQuarters;
                        default:
                            return -1;
                    }
                }
            }
            public int GoodQuantity
            {
                get
                {
                    int s;
                    switch (denomination)
                    {
                        
                        case CloudCoin.Denomination.One:
                            s = 0;
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Authenticated && coin.denomination == CloudCoin.Denomination.One)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Five:
                            s = 0;
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Authenticated && coin.denomination == CloudCoin.Denomination.Five)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Quarter:
                            s = 0;
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Authenticated && coin.denomination == CloudCoin.Denomination.Quarter)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Hundred:
                            s = 0;
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Authenticated && coin.denomination == CloudCoin.Denomination.Hundred)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.KiloQuarter:
                            s = 0;
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Authenticated && coin.denomination == CloudCoin.Denomination.KiloQuarter)
                                    s++;
                            }
                            return s;
                        default:
                            return -1;
                    }
                    
                }
            }
            public int FractionedQuantity
            {
                get
                {
                    int s = 0;
                    switch (denomination)
                    {
                        case CloudCoin.Denomination.One:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Fractioned && coin.denomination == CloudCoin.Denomination.One)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Five:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Fractioned && coin.denomination == CloudCoin.Denomination.Five)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Quarter:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Fractioned && coin.denomination == CloudCoin.Denomination.Quarter)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Hundred:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Fractioned && coin.denomination == CloudCoin.Denomination.Hundred)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.KiloQuarter:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Fractioned && coin.denomination == CloudCoin.Denomination.KiloQuarter)
                                    s++;
                            }
                            return s;
                        default:
                            return -1;
                    }

                }
            }
            public int CounterfeitedQuantity
            {
                get
                {
                    int s = 0;
                    switch (denomination)
                    {
                        case CloudCoin.Denomination.One:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Counterfeit && coin.denomination == CloudCoin.Denomination.One)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Five:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Counterfeit && coin.denomination == CloudCoin.Denomination.Five)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Quarter:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Counterfeit && coin.denomination == CloudCoin.Denomination.Quarter)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.Hundred:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Counterfeit && coin.denomination == CloudCoin.Denomination.Hundred)
                                    s++;
                            }
                            return s;
                        case CloudCoin.Denomination.KiloQuarter:
                            foreach (CloudCoin coin in current.Contents)
                            {
                                if (coin.Verdict == CloudCoin.Status.Counterfeit && coin.denomination == CloudCoin.Denomination.KiloQuarter)
                                    s++;
                            }
                            return s;
                        default:
                            return -1;
                    }

                }
            }

            public Shelf(Safe safe, CloudCoin.Denomination denom)
            {
                current = safe;
                denomination = denom;
            }
        }

        private Safe()
        {
            var settingsSafeFilePath =  Properties.Settings.Default.SafeFileName;
            safeFilePath = Environment.ExpandEnvironmentVariables(settingsSafeFilePath);
            
            safeFileInfo = new FileInfo(safeFilePath);
            if (!safeFileInfo.Exists)
            {
                SetPassword();
                Contents = new CoinStack();
                CreateSafeFile();
            }
            else
            {
                CheckPassword();
                ReadSafeFile();
            }
        }

        private void SetPassword()
        {
            var passwordWindow = new SetPassword();
            passwordWindow.Password.Focus();
            passwordWindow.ShowDialog();
            password = passwordWindow.Password.Password;
            cryptedPass = Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(password));
        }

        private void CheckPassword()
        {
            if(password == null)
            {
                using (var fs = safeFileInfo.Open(FileMode.Open))
                {
                    byte[] buffer = new byte[60];
                    fs.Read(buffer, 0, 60);
                    string cryptedPassSafe = new string(Encoding.UTF8.GetChars(buffer));
                    var enterPassword = new EnterPassword();
                    enterPassword.passwordBox.Focus();
                    enterPassword.ShowDialog();
                    byte[] passbytes = Encoding.UTF8.GetBytes(enterPassword.passwordBox.Password);
                    while (!Crypter.CheckPassword(passbytes, cryptedPassSafe))
                    {
                        MessageBox.Show("Wrong password from safe.\nTry again.");
                        enterPassword.ShowDialog();
                        passbytes = Encoding.UTF8.GetBytes(enterPassword.passwordBox.Password);
                    }
                    password = enterPassword.passwordBox.Password;
                    cryptedPass = cryptedPassSafe;
                    enterPassword.Close();
                }
            }
        }

        private void CreateSafeFile()
        {

            byte[] cryptedpassbytes = Encoding.UTF8.GetBytes(cryptedPass);
            Directory.CreateDirectory(safeFileInfo.DirectoryName);
            try
            {
                var json = JsonConvert.SerializeObject(Contents);
                var cryptedjson = Convert.Encrypt(json, password, cryptedpassbytes.Take(16).ToArray());
                using (var fs = safeFileInfo.Create())
                {
                    fs.Write(cryptedpassbytes, 0, 60);
                    fs.Write(cryptedjson, 0, cryptedjson.Length);
                }
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() JSON exception: " + ex.Message);
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() encryption exception: " + ex.Message);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() IO write exception: " + ex.Message);
            }
        }

        private void ReadSafeFile()
        {
            using (var fs = safeFileInfo.Open(FileMode.Open))
            {

                string json = null;
                byte[] cryptedjson = new byte[(int)(fs.Length - 60)];
                try
                {
                    var numbytestoread = fs.Length - 60;
                    int numbytesread = 0;
                    fs.Position = 60;
                    while (true)
                    {
                        var n = fs.Read(cryptedjson, numbytesread, (int)numbytestoread);
                        if (n == 0) break;
                        numbytesread += n;
                        numbytestoread -= n;
                    }
                    json = Convert.Decrypt(cryptedjson, password, Encoding.UTF8.GetBytes(cryptedPass).Take(16).ToArray());
                    Contents = JsonConvert.DeserializeObject<CoinStack>(json);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() IO read exception: " + ex.Message);
                }
                catch (CryptographicException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() decrypting exception: " + ex.Message);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() JSON deserialize exception: " + ex.Message);
                }
            }
        }

        public void Add(CoinStack stack)
        {
            safeFileInfo.Refresh();
            if (!safeFileInfo.Exists)
                return;
            using (var fs = safeFileInfo.Open(FileMode.Open))
            {
                byte[] cryptedjsonbytes = null;
                Contents.Add(stack);
                Contents.RemoveCounterfeitCoins();
                Contents.cloudcoin.Sort(new CloudCoin.CoinComparer());
                string jsonstring = "";
                try
                {
                    jsonstring = JsonConvert.SerializeObject(Contents);
                    cryptedjsonbytes = Convert.Encrypt(jsonstring, password, Encoding.UTF8.GetBytes(cryptedPass).Take(16).ToArray());
                    fs.Write(Encoding.UTF8.GetBytes(cryptedPass), 0, 60);
                    fs.Write(cryptedjsonbytes, 0, cryptedjsonbytes.Length);
                }
                catch (JsonException e)
                {
                    MessageBox.Show("Safe.Add() JSON deserialize exception: " + e.Message);
                }
                catch (CryptographicException ex)
                {
                    MessageBox.Show("Safe.Add() decrypting exception: " + ex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Safe.Add() IO write exception: " + ex.Message);
                }
            }
        }

        public void SaveOutStack()
        {
            var howMuch = new HowMuchWindow();
            howMuch.enterSumBox.Focus();
            howMuch.ShowDialog();
            short desiredSum = short.Parse(howMuch.enterSumBox.Text);
            CoinStack stack = ChooseNearestPossibleStack(desiredSum);
            DateTime currdate = DateTime.Now;
            stack.SaveInFile(Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinDir) + 
                currdate.ToString("dd-MM-yy_HH-mm")+".ccstack");
        }

        private CoinStack ChooseNearestPossibleStack(short sum)
        {
            return new CoinStack();
        }
        public void Show()
        {
            var safeWindow = new SafeContents();

            safeWindow.Show();
            safeWindow.totalTextBox.Text = Contents.SumInStack.ToString() + " CC in Safe";
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "Ones", Good = Ones.GoodQuantity,
                Fractioned = Ones.FractionedQuantity, Counterfeited = Ones.CounterfeitedQuantity, Total = Ones.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "Fives", Good = Fives.GoodQuantity,
                Fractioned = Fives.FractionedQuantity, Counterfeited = Fives.CounterfeitedQuantity, Total = Fives.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "Quarters", Good = Quarters.GoodQuantity,
                Fractioned = Quarters.FractionedQuantity, Counterfeited = Quarters.CounterfeitedQuantity, Total = Quarters.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "Hundreds", Good = Hundreds.GoodQuantity,
                Fractioned = Hundreds.FractionedQuantity, Counterfeited = Hundreds.CounterfeitedQuantity, Total = Hundreds.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "250s", Good = KiloQuarters.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity, Counterfeited = KiloQuarters.CounterfeitedQuantity, Total = KiloQuarters.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContents.Shelf4Display() { Value = "Sum:",
                Good = KiloQuarters.GoodQuantity*250+Hundreds.GoodQuantity*100+Quarters.GoodQuantity*25+Fives.GoodQuantity*5+Ones.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity*250+Hundreds.FractionedQuantity*100+Quarters.FractionedQuantity*25+Fives.FractionedQuantity*5+Ones.FractionedQuantity,
                Counterfeited = KiloQuarters.CounterfeitedQuantity*250+Hundreds.CounterfeitedQuantity*100+Quarters.CounterfeitedQuantity*25+Fives.CounterfeitedQuantity*5+Ones.CounterfeitedQuantity,
                Total = KiloQuarters.TotalQuantity*250+Hundreds.TotalQuantity*100+Quarters.TotalQuantity*25+Fives.TotalQuantity*5+Ones.TotalQuantity });
        }
    }


}
