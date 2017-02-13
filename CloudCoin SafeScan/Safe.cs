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
            var passwordWindow = new SetPasswordWindow();
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
                    var enterPassword = new EnterPasswordWindow();
                    enterPassword.Owner = App.Current.MainWindow;
                    enterPassword.passwordBox.Focus();
                    enterPassword.ShowDialog();
                    byte[] passbytes = Encoding.UTF8.GetBytes(enterPassword.passwordBox.Password);
                    while (!Crypter.CheckPassword(passbytes, cryptedPassSafe))
                    {
                        MessageBox.Show("Wrong password from safe.\nTry again.");
                        var x = enterPassword ?? new EnterPasswordWindow(); // The window might be closed
                        x.ShowDialog();
                        passbytes = Encoding.UTF8.GetBytes(x.passwordBox.Password);
                    }
                    password = Encoding.UTF8.GetString(passbytes);
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
            Contents.Add(stack);
            //RemoveCounterfeitCoins();
            Contents.cloudcoin.Sort(new CloudCoin.CoinComparer());
            Save();
        }

        private void Save()
        {
            safeFileInfo.Refresh();
            if (!safeFileInfo.Exists)
                return;

            using (var fs = safeFileInfo.Open(FileMode.Open))
            {
                byte[] cryptedjsonbytes = null;
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

        private void RemoveCounterfeitCoins()
        {
            Contents.cloudcoin.RemoveAll(delegate (CloudCoin coin) { return coin.Verdict == CloudCoin.Status.Counterfeit; });
        }

        public void SaveOutStack()
        {
            var howMuch = new HowMuchWindow();
            howMuch.enterSumBox.Focus();
            howMuch.ShowDialog();
            int desiredSum = int.Parse(howMuch.enterSumBox.Text);
            CoinStack stack = ChooseNearestPossibleStack(desiredSum);
            CoinStackOut st = new CoinStackOut(stack);
            DateTime currdate = DateTime.Now;
            string fn = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinDir) +
                currdate.ToString("dd-MM-yy_HH-mm") + ".ccstack";
            st.SaveInFile(fn);
            MessageBox.Show("Stack saved in file \n" + fn);
        }

        private CoinStack ChooseNearestPossibleStack(int sum)
        {
            // define short variables for convinience
            var csc = Instance.Contents;
            int kQ=0, h=0, q=0, f=0, o=0;
            
            if (sum > 250 && csc.KiloQuarters > 0) //are there any coins of such denomination?
            {
                // choose maximum of coins of such denomnation
                kQ = (csc.KiloQuarters > sum / 250) ? sum / 250 : csc.KiloQuarters;
                sum -= kQ * 250;
            }
            if(sum > 100 && csc.Hundreds > 0)
            {
                h = (csc.Hundreds > sum / 100) ? sum / 100 : csc.Hundreds;
                sum -= h * 100;
            }
            if (sum > 25 && csc.Quarters > 0)
            {
                q = (csc.Quarters > sum / 25) ? sum / 25 : csc.Quarters;
                sum -= q * 25;
            }
            if (sum > 5 && csc.Fives > 0)
            {
                f = (csc.Fives > sum / 5) ? sum / 5 : csc.Fives;
                sum -= f * 5;
            }
            if (sum > 1 && csc.Ones > 0)
            {
                o = (csc.Ones > sum) ? sum : csc.Ones;
                sum -= o;
            }
            //show which will form stack >= requested sum
            var selectWindow = new SelectOutStackWindow();
            selectWindow.stacksToSelect.Items.Add(new SelectOutStackWindow.Stack4Display()
            { Ones = o, Fives = f, Quarters = q, Hundreds = h, KiloQuarters = kQ, Total = (o + f * 5 + q * 25 + h * 100 + kQ * 250) });
            //adding existing coin of minimal denomination to form second choice which will be greater than requested sum
            if (sum > 0)
            {
                if ((csc.Ones - o) > 0) o++;
                else if ((csc.Fives - f) > 0) f++;
                else if ((csc.Quarters - q) > 0) q++;
                else if ((csc.Hundreds - h) > 0) h++;
                else if ((csc.Quarters - kQ) > 0) kQ++;
                selectWindow.stacksToSelect.Items.Add(new SelectOutStackWindow.Stack4Display()
                { Ones = o, Fives = f, Quarters = q, Hundreds = h, KiloQuarters = kQ, Total = (o + f * 5 + q * 25 + h * 100 + kQ * 250) });
            }
            selectWindow.ShowDialog();
            SelectOutStackWindow.Stack4Display res = (SelectOutStackWindow.Stack4Display) selectWindow.stacksToSelect.SelectedItem;

            List<CloudCoin> tmp = new List<CloudCoin>(o+f+q+h+kQ);
            IEnumerable<IGrouping<CloudCoin.Denomination, CloudCoin>> GroupsCoinQuery = from coin in csc.cloudcoin
                                                   group coin by coin.denomination;

            foreach (var cG in GroupsCoinQuery)
            {
                int count = 0;
                
                switch (cG.Key)
                {
                    case CloudCoin.Denomination.One:
                        count = res.Ones;
                        break;
                    case CloudCoin.Denomination.Five:
                        count = res.Fives;
                        break;
                    case CloudCoin.Denomination.Quarter:
                        count = res.Quarters;
                        break;
                    case CloudCoin.Denomination.Hundred:
                        count = res.Hundreds;
                        break;
                    case CloudCoin.Denomination.KiloQuarter:
                        count = res.KiloQuarters;
                        break;
                }
                foreach (CloudCoin c in cG.Take(count))
                {
                    tmp.Add(c);
                    csc.cloudcoin.Remove(c);
                }
            } 
            var result = new CoinStack();
            result.cloudcoin.AddRange(tmp);
            return result;
        }

        public void Show()
        {
            var safeWindow = new SafeContentWindow();

            safeWindow.Show();
            safeWindow.totalTextBox.Text = Contents.SumInStack.ToString() + " CC in Safe";
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Ones", Good = Ones.GoodQuantity,
                Fractioned = Ones.FractionedQuantity, Counterfeited = Ones.CounterfeitedQuantity, Total = Ones.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Fives", Good = Fives.GoodQuantity,
                Fractioned = Fives.FractionedQuantity, Counterfeited = Fives.CounterfeitedQuantity, Total = Fives.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Quarters", Good = Quarters.GoodQuantity,
                Fractioned = Quarters.FractionedQuantity, Counterfeited = Quarters.CounterfeitedQuantity, Total = Quarters.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Hundreds", Good = Hundreds.GoodQuantity,
                Fractioned = Hundreds.FractionedQuantity, Counterfeited = Hundreds.CounterfeitedQuantity, Total = Hundreds.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "250s", Good = KiloQuarters.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity, Counterfeited = KiloQuarters.CounterfeitedQuantity, Total = KiloQuarters.TotalQuantity });
            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Sum:",
                Good = KiloQuarters.GoodQuantity*250+Hundreds.GoodQuantity*100+Quarters.GoodQuantity*25+Fives.GoodQuantity*5+Ones.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity*250+Hundreds.FractionedQuantity*100+Quarters.FractionedQuantity*25+Fives.FractionedQuantity*5+Ones.FractionedQuantity,
                Counterfeited = KiloQuarters.CounterfeitedQuantity*250+Hundreds.CounterfeitedQuantity*100+Quarters.CounterfeitedQuantity*25+Fives.CounterfeitedQuantity*5+Ones.CounterfeitedQuantity,
                Total = KiloQuarters.TotalQuantity*250+Hundreds.TotalQuantity*100+Quarters.TotalQuantity*25+Fives.TotalQuantity*5+Ones.TotalQuantity });
        }
    }


}
