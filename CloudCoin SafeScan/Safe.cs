using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using CryptSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public class Safe
    {
        private const string SLOGAN = "Не в силе Бог, а в правде!";
        //Singleton instance could be object or null, must check in every call
        public static Safe Instance
        {
            get
            {
                return theOnlySafeInstance ?? GetInstance();   //Singleton Fabric
            }

        }
        private static Safe theOnlySafeInstance = null;

        //Static fields
        private static string cryptPassFromFile = ""; //encrypted string which has been read from Safe file
        private static string userEnteredPassword;
        private static byte[] encryptedUserEnteredPassword;
        private static byte[] salt = Encoding.UTF8.GetBytes(SLOGAN);

        private static Safe GetInstance()
        {
            string settingsSafeFilePath = Properties.Settings.Default.SafeFileName;
            string filePath = Environment.ExpandEnvironmentVariables(settingsSafeFilePath);
            string bkpFilePath = filePath + ".bkp";

            var fileInfo = new FileInfo(filePath);
            var bkpFileInfo = new FileInfo(bkpFilePath);
            if (!bkpFileInfo.Exists)
            {
                var coins = new CoinStack();
                using (var tmp = bkpFileInfo.Create())
                {
                    tmp.Close();
                }

            }
            if (!fileInfo.Exists)
            { //Safe does not exist, create one
                userEnteredPassword = UserInteract.SetPassword(); //get user password for Safe
                if (userEnteredPassword != "error")
                {
                    encryptedUserEnteredPassword = Encoding.UTF8.GetBytes(Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(userEnteredPassword)));
                    var coins = new CoinStack();
                    if (CreateSafeFile(fileInfo, coins))
                    {
                        theOnlySafeInstance = new Safe(fileInfo, coins);
                        return theOnlySafeInstance;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else //Safe already exists
            {
                userEnteredPassword = UserInteract.CheckPassword(fileInfo); //get user password and check against stored in file
                if (userEnteredPassword != "error")
                {
                    encryptedUserEnteredPassword = Encoding.UTF8.GetBytes(Crypter.Blowfish.Crypt(Encoding.UTF8.GetBytes(userEnteredPassword)));
                    CoinStack safeContents = ReadSafeFile(fileInfo);
                    if (safeContents != null)
                    {
                        theOnlySafeInstance = new Safe(fileInfo, safeContents);
                        return theOnlySafeInstance;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        private static bool CreateSafeFile(FileInfo fi, CoinStack stack)
        {
            try
            {
                Directory.CreateDirectory(fi.DirectoryName);
                var json = JsonConvert.SerializeObject(stack);
                var cryptedjson = Utils.Encrypt(json, userEnteredPassword, salt);
                using (var fs = fi.Create())
                {
                    fs.Write(encryptedUserEnteredPassword, 0, 60);
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

        private static CoinStack ReadSafeFile(FileInfo fi)
        {
            using (var fs = fi.Open(FileMode.Open))
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
                    json = Utils.Decrypt(cryptedjson, userEnteredPassword, salt);
                    var stack = JsonConvert.DeserializeObject<CoinStack>(json, new JsonSerializerSettings { CheckAdditionalContent = false } );
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

        public event SafeContentChangedEventHandler SafeChanged;
        public void onSafeContentChanged(EventArgs e)
        {
            SafeChanged?.Invoke(this, e);
        }

        private string safeFilePath;
        private string bkpFilePath;
        private FileInfo safeFileInfo;
        private FileInfo bkpFileInfo;
        public CoinStack Contents; // contents of the Safe
        public List<CloudCoin> FrackedCoinsList
        {
            get { return Instance.Contents.cloudcoin.FindAll(x => x.Verdict == CloudCoin.Status.Fractioned); }
        }
        public Shelf Ones
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.One);
            }
        } //Shelf with denomination 1 coins
        public Shelf Fives
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Five);
            }
        } //Shelf with denomination 5 coins
        public Shelf Quarters
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Quarter);
            }
        } //Shelf with denomination 25 coins
        public Shelf Hundreds
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.Hundred);
            }
        } //Shelf with denomination 100 coins
        public Shelf KiloQuarters
        {
            get
            {
                return new Shelf(this, CloudCoin.Denomination.KiloQuarter);
            }
        } //Shelf with denomination 250 coins

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

        private Safe(FileInfo fi, CoinStack coins)
        {
            safeFilePath = fi.FullName;
            bkpFilePath = fi.FullName + ".bkp";
            safeFileInfo = fi;
            bkpFileInfo = new FileInfo(bkpFilePath);
            Contents = coins;
        }

        public void Add(CoinStack stack)
        {
            Contents.Add(stack);
            RemoveCounterfeitCoins();
            onSafeContentChanged(new EventArgs());
            Save();
        }

        public void Remove(CloudCoin coin)
        {
            Contents.cloudcoin.Remove(coin);
            onSafeContentChanged(new EventArgs());
            Save();
        }

        public void Save()
        {
            safeFileInfo.Refresh();
            if (!safeFileInfo.Exists)
                return;
            Contents.Distinct();
            using (var fs = safeFileInfo.Open(FileMode.Open))
            {
                byte[] cryptedjsonbytes = null;
                string jsonstring = "";
                try
                {
                    jsonstring = JsonConvert.SerializeObject(Contents);
                    cryptedjsonbytes = Utils.Encrypt(jsonstring, userEnteredPassword, salt);
                    fs.Write(encryptedUserEnteredPassword, 0, 60);
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
            using (var fs = bkpFileInfo.Open(FileMode.Open))
            {
                byte[] cryptedjsonbytes = null;
                string jsonstring = "";
                try
                {
                    jsonstring = JsonConvert.SerializeObject(Contents);
                    cryptedjsonbytes = Utils.Encrypt(jsonstring, userEnteredPassword, salt);
                    fs.Write(encryptedUserEnteredPassword, 0, 60);
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

        public void SaveOutStack(int desiredSum)
        {
            CoinStack stack = ChooseNearestPossibleStack(desiredSum);
            if (stack != null)
            {
                onSafeContentChanged(new EventArgs());
                CoinStackOut st = new CoinStackOut(stack);
                string fn = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinExportDir) +
                    DateTime.Now.ToString("dd-MM-yy_HH-mm") + ".ccstack";
                st.SaveInFile(fn);
                Logger.Write("Exported stack with " + stack.cloudcoin.Count + " coins.", Logger.Level.Normal);
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
            selectWindow.stacksToSelect.Items.Add(new SelectOutStackWindowViewModel()
            { Ones = o, Fives = f, Quarters = q, Hundreds = h, KiloQuarters = kQ, Total = (o + f * 5 + q * 25 + h * 100 + kQ * 250) });
            //adding existing coin of minimal denomination to form second choice which will be greater than requested sum
            if (sum > 0)
            {
                if ((csc.Ones - o) > 0) o++;
                else if ((csc.Fives - f) > 0) f++;
                else if ((csc.Quarters - q) > 0) q++;
                else if ((csc.Hundreds - h) > 0) h++;
                else if ((csc.KiloQuarters - kQ) > 0) kQ++;
                selectWindow.stacksToSelect.Items.Add(new SelectOutStackWindowViewModel()
                { Ones = o, Fives = f, Quarters = q, Hundreds = h, KiloQuarters = kQ, Total = (o + f * 5 + q * 25 + h * 100 + kQ * 250) });
            }
            selectWindow.stacksToSelect.SelectedItem = selectWindow.stacksToSelect.Items[0];
            selectWindow.ShowDialog();
            if (selectWindow.DialogResult == true)
            {
                SelectOutStackWindowViewModel res = (SelectOutStackWindowViewModel)selectWindow.stacksToSelect.SelectedItem;

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
                        Remove(c);
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
            (new SafeContentWindow()).Show();
            Save();
        }
    }
}
