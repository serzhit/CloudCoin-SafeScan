using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using CryptSharp;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public class Safe
    {
        public static Safe Instance
        {
            get
            {
                return theOnlySafeInstance ?? GetInstance();
            }

        }

        private static Safe theOnlySafeInstance = null;
        private static Safe GetInstance()
        {
            var settingsSafeFilePath = Properties.Settings.Default.SafeFileName;
            var filePath = Environment.ExpandEnvironmentVariables(settingsSafeFilePath);

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            { //Safe does not exist, create one
                var pass = SetPassword();
                if (pass != "error")
                {
                    var coins = new CoinStack();
                    if (CreateSafeFile(fileInfo, pass, coins))
                    {
                        theOnlySafeInstance = new Safe(fileInfo, pass, coins);
                        return theOnlySafeInstance;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
            {
                var pass = CheckPassword(fileInfo);
                if (pass != "error")
                {
                    CoinStack safeContents = ReadSafeFile(fileInfo, pass);
                    if (safeContents != null)
                    {
                        theOnlySafeInstance = new Safe(fileInfo, pass, safeContents);
                        return theOnlySafeInstance;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        private static string cryptPassFromFile = "";
        public string safeFilePath;
        public FileInfo safeFileInfo;
        private string password;
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

        private Safe(FileInfo fi, string pass, CoinStack coins)
        {
            password = pass;
            safeFilePath = fi.FullName;
            safeFileInfo = fi;
            cryptedPass = Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(pass)); ;
            Contents = coins;
            beforeFixStart += new EventHandler(StartFixProcess);
        }

        private static string SetPassword()
        {
            var passwordWindow = new SetPasswordWindow();
            passwordWindow.Password.Focus();
            passwordWindow.Owner = MainWindow.Instance;
            passwordWindow.ShowDialog();
            if (passwordWindow.DialogResult == true)
            {
                return passwordWindow.Password.Password;
            }
            else
                return "error";
        }

        private static string CheckPassword(FileInfo fi)
        {
            using (var fs = fi.Open(FileMode.Open))
            {
                byte[] buffer = new byte[60];
                byte[] passbytes = { 1, 2, 3, 4 }; //bogus data just to initialize
                fs.Read(buffer, 0, 60);
                cryptPassFromFile = new string(Encoding.UTF8.GetChars(buffer));
                while (true)
                {
                    var enterPassword = new EnterPasswordWindow();
                    enterPassword.Owner = MainWindow.Instance;
                    enterPassword.passwordBox.Focus();
                    enterPassword.ShowDialog();
                    if (enterPassword.DialogResult == true)
                    {
                        passbytes = Encoding.UTF8.GetBytes(enterPassword.passwordBox.Password);
                        if (Crypter.CheckPassword(passbytes, cryptPassFromFile))
                        {
                            enterPassword.Close();
                            return enterPassword.passwordBox.Password;
                        }
                        else
                        {
                            MessageBox.Show("Wrong password from safe.\nTry again.");
                            enterPassword.Close();
                        }
                    }
                    else
                    {
                        enterPassword.Close();
                        break;
                    }
                }
            }
            return "error";
        }
        
        private static bool CreateSafeFile(FileInfo fi, string pass, CoinStack stack)
        {
            var cryptpass = Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(pass));
            byte[] cryptedpassbytes = Encoding.UTF8.GetBytes(cryptpass);
            try
            {
                Directory.CreateDirectory(fi.DirectoryName);
                var json = JsonConvert.SerializeObject(stack);
                var cryptedjson = Utils.Encrypt(json, pass, cryptedpassbytes.Take(16).ToArray());
                using (var fs = fi.Create())
                {
                    fs.Write(cryptedpassbytes, 0, 60);
                    fs.Write(cryptedjson, 0, cryptedjson.Length);
                }
                return true;
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() JSON exception: " + ex.Message);
                return false;
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() encryption exception: " + ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Safe.CreateSafeFile() IO write exception: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static CoinStack ReadSafeFile(FileInfo fi, string pass)
        {
//            var cryptpass = Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(pass));
//            byte[] cryptedpassbytes = Encoding.UTF8.GetBytes(cryptpass);
            using (var fs = fi.Open(FileMode.Open))
            {
                byte[] cryptedbytes = Encoding.UTF8.GetBytes(cryptPassFromFile);
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
                    json = Utils.Decrypt(cryptedjson, pass, cryptedbytes.Take(16).ToArray());
                    var stack = JsonConvert.DeserializeObject<CoinStack>(json);
                    return stack;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() IO read exception: " + ex.Message);
                    return null;
                }
                catch (CryptographicException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() decrypting exception: " + ex.Message);
                    return null;
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Safe.ReadSafeFile() JSON deserialize exception: " + ex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public void Add(CoinStack stack)
        {
            Contents.Add(stack);
            RemoveCounterfeitCoins();
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
                    cryptedjsonbytes = Utils.Encrypt(jsonstring, password, Encoding.UTF8.GetBytes(cryptedPass).Take(16).ToArray());
                    fs.Write(Encoding.UTF8.GetBytes(cryptedPass), 0, 60);
                    fs.Write(cryptedjsonbytes, 0, cryptedjsonbytes.Length);
                }
                catch (JsonException e)
                {
                    MessageBox.Show("Safe.Add() JSON serialize exception: " + e.Message);
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

        public event EventHandler beforeFixStart;
        public virtual void onBeforeFixStart(EventArgs e)
        {
            beforeFixStart?.Invoke(this, e);
        }
        private void StartFixProcess(object sender, EventArgs e)
        {
            List<CloudCoin> coinsToFix = Contents.cloudcoin.FindAll(x => x.Verdict == CloudCoin.Status.Fractioned);
            FixCoinWindow fixWin;
            if (coinsToFix.Count > 0)
            {
                
                RecurrentFix(coinsToFix);
            }
        }

        private void RecurrentFix(List<CloudCoin> coinstofix)
        {
            FixCoinWindow fixWin = new FixCoinWindow();
            if (coinstofix.Count > 0)
            {
                var coin = coinstofix[0];
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    fixWin.Load(coin);
                    fixWin.Show();
                });
                var fixCoinTask = new Task(() => RAIDA.Instance.fixCoin(coin, fixWin));
                fixCoinTask.Start();
                fixCoinTask.Wait();
                Save();
                coinstofix.Remove(coin);
                RecurrentFix(coinstofix);
                /*                fixCoinTask.ContinueWith((ancestor) =>
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Save();
                                    });
                                    coinstofix.Remove(coin);
                                    RecurrentFix(coinstofix, fixWin);
                                }, TaskContinuationOptions.ExecuteSynchronously);
                */

            }
        }

        public void TryFix()
        {
            List<CloudCoin> coinsToFix = Contents.cloudcoin.FindAll(x => x.Verdict == CloudCoin.Status.Fractioned);

            foreach(var coin in coinsToFix)
            {
//                RAIDA.Instance.fixCoin(coin);
            }
            Save();
        }
        public void SaveOutStack()
        {
            var howMuch = new HowMuchWindow();
            howMuch.enterSumBox.Focus();
            howMuch.Owner = MainWindow.Instance;
            howMuch.ShowDialog();
            if (howMuch.DialogResult == true)
            {
                int desiredSum = int.Parse(howMuch.enterSumBox.Text);
                CoinStack stack = ChooseNearestPossibleStack(desiredSum);
                if (stack != null)
                {
                    CoinStackOut st = new CoinStackOut(stack);
                    DateTime currdate = DateTime.Now;
                    string fn = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinDir) +
                        currdate.ToString("dd-MM-yy_HH-mm") + ".ccstack";
                    st.SaveInFile(fn);
                    MessageBox.Show("Stack saved in file \n" + fn);
                }
            }
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
            selectWindow.Owner = MainWindow.Instance;
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
            selectWindow.stacksToSelect.SelectedItem = selectWindow.stacksToSelect.Items[0];
            selectWindow.ShowDialog();
            if (selectWindow.DialogResult == true)
            {
                SelectOutStackWindow.Stack4Display res = (SelectOutStackWindow.Stack4Display)selectWindow.stacksToSelect.SelectedItem;

                List<CloudCoin> tmp = new List<CloudCoin>(o + f + q + h + kQ);
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
            else
                return null;
        }

        public void Show()
        {
            var safeWindow = new SafeContentWindow();

            safeWindow.Owner = MainWindow.Instance;
            safeWindow.Show();
            safeWindow.totalTextBox.Text = Contents.SumInStack.ToString() + " CC in Safe";
            SafeContentWindow.Shelf4Display[] items = new SafeContentWindow.Shelf4Display[6]
            {
                new SafeContentWindow.Shelf4Display() { Value = "Ones", Good = Ones.GoodQuantity,
                Fractioned = Ones.FractionedQuantity, Counterfeited = Ones.CounterfeitedQuantity, Total = Ones.TotalQuantity },
                new SafeContentWindow.Shelf4Display() { Value = "Fives", Good = Fives.GoodQuantity,
                Fractioned = Fives.FractionedQuantity, Counterfeited = Fives.CounterfeitedQuantity, Total = Fives.TotalQuantity },
                new SafeContentWindow.Shelf4Display() { Value = "Quarters", Good = Quarters.GoodQuantity,
                Fractioned = Quarters.FractionedQuantity, Counterfeited = Quarters.CounterfeitedQuantity, Total = Quarters.TotalQuantity },
                new SafeContentWindow.Shelf4Display() { Value = "Hundreds", Good = Hundreds.GoodQuantity,
                Fractioned = Hundreds.FractionedQuantity, Counterfeited = Hundreds.CounterfeitedQuantity, Total = Hundreds.TotalQuantity },
                new SafeContentWindow.Shelf4Display() { Value = "250s", Good = KiloQuarters.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity, Counterfeited = KiloQuarters.CounterfeitedQuantity, Total = KiloQuarters.TotalQuantity },
                new SafeContentWindow.Shelf4Display() { Value = "Sum:",
                Good = KiloQuarters.GoodQuantity*250+Hundreds.GoodQuantity*100+Quarters.GoodQuantity*25+Fives.GoodQuantity*5+Ones.GoodQuantity,
                Fractioned = KiloQuarters.FractionedQuantity*250+Hundreds.FractionedQuantity*100+Quarters.FractionedQuantity*25+Fives.FractionedQuantity*5+Ones.FractionedQuantity,
                Counterfeited = KiloQuarters.CounterfeitedQuantity*250+Hundreds.CounterfeitedQuantity*100+Quarters.CounterfeitedQuantity*25+Fives.CounterfeitedQuantity*5+Ones.CounterfeitedQuantity,
                Total = KiloQuarters.TotalQuantity*250+Hundreds.TotalQuantity*100+Quarters.TotalQuantity*25+Fives.TotalQuantity*5+Ones.TotalQuantity }
            };
            safeWindow.SafeView.ItemsSource = items;
/*            safeWindow.SafeView.Items.Add(new SafeContentWindow.Shelf4Display() { Value = "Ones", Good = Ones.GoodQuantity,
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
*/
        }
    }
}
