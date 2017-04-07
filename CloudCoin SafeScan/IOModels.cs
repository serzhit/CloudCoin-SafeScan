using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace CloudCoin_SafeScan
{
    #region
    public static class FileSystem
    {
        internal static void InitializePaths()
        {
            string homedir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinDir);
            string importdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinImportDir);
            string exportdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinExportDir);
            string backupdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinBackupDir);
            string logdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinLogDir);

            foreach (string path in new string[] { homedir, importdir, exportdir, backupdir, logdir })
            {
                DirectoryInfo DI = new DirectoryInfo(path);
                if (!DI.Exists)
                {
                    DI.Create();
                }
            }
        }

        internal static void CopyOriginalFileToImported(FileInfo FI)
        {
            string dt = DateTime.Now.ToString("dd-MM-yy_HH-mm");
            string importdir = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinImportDir);
            var newFileName = importdir + FI.Name + ".imported-" + dt;
            File.Copy(FI.FullName, newFileName);
        }

        public static string[] ChooseInputFile()
        {
            OpenFileDialog FD = new OpenFileDialog();
            FD.Multiselect = true;
            FD.Title = "Choose file with Cloudcoin(s)";
            FD.InitialDirectory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.UserCloudcoinImportDir);
            if (FD.ShowDialog() == true)
            {
                return FD.FileNames;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
    #endregion
    public class CloudCoinFile
    {
        public enum Type { json, jpeg, unknown }
        public Type Filetype;
        string Filename;
        FileInfo FI;
        public CoinStack Coins = new CoinStack();

        public CloudCoinFile(string[] names)
        {
            foreach (string path in names)
            {
                ParseCloudCoinFile(path);
            }
        }

        private void ParseCloudCoinFile(string fullPath)
        {
            FI = new FileInfo(fullPath);
            if (FI.Exists)
            {
                Filename = fullPath;
                using (Stream fsSource = FI.Open(FileMode.Open))
                {
                    byte[] signature = new byte[20];
                    fsSource.Read(signature, 0, 20);
                    string sig = Encoding.UTF8.GetString(signature);
                    var reg = new Regex(@"{[.\n\t\s\x09\x0A\x0D]*""cloudcoin""");
                    if (Enumerable.SequenceEqual(signature.Take(3), new byte[] { 255, 216, 255 })) //JPEG
                    {
                        Filetype = Type.jpeg;
                        var coin = ReadJpeg(fsSource);
                        if (coin != null)
                        {
                            Coins.Add(new CoinStack(coin));
                        }
                    }
                    else if (reg.IsMatch(sig)) //JSON
                    {
                        Filetype = Type.json;
                        var json = ReadJson(fsSource);
                        if (json != null)
                        {
                            Coins.Add(json);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MainWindow.Instance, Filename + "\n: does not contain CloudCoins!");
                    }
                }
                FileSystem.CopyOriginalFileToImported(FI);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private CloudCoin ReadJpeg(Stream jpegFS)
        {
            // TODO: catch exception for wrong file format
            //            filetype = Type.jpeg;
            byte[] fileByteContent = new byte[455];
            int numBytesToRead = fileByteContent.Length;
            int numBytesRead = 0;
            string[] an = new string[RAIDA.NODEQNTY];
            string[] aoid = new string[1];
            int sn;
            int nn;
            string ed;

            jpegFS.Position = 0;
            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = jpegFS.Read(fileByteContent, numBytesRead, numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

            string jpegHexContent = "";
            jpegHexContent = Utils.ToHexString(fileByteContent);

            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                an[i] = jpegHexContent.Substring(40 + i * 32, 32);
            }
            aoid[0] = jpegHexContent.Substring(840, 55);
            ed = jpegHexContent.Substring(898, 4);
            nn = Int16.Parse(jpegHexContent.Substring(902, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            sn = Int32.Parse(jpegHexContent.Substring(904, 6), System.Globalization.NumberStyles.AllowHexSpecifier);

            CloudCoin coin = new CloudCoin(nn, sn, an, ed, aoid);
            if (coin.Calibrate())
            {
                return coin;
            }
            return null;
        }

        private CoinStack ReadJson(Stream jsonFS)
        {
            jsonFS.Position = 0;
            StreamReader sr = new StreamReader(jsonFS);
            CoinStack stack = null;
            try
            {
                stack = JsonConvert.DeserializeObject<CoinStack>(sr.ReadToEnd());
            }
            catch (JsonException ex)
            {
                throw;
            }
            return stack;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CloudCoinOut
    {
        [JsonProperty(Order = 1)]
        public string nn { get; set; }
        [JsonProperty(Order = 2)]
        public string sn { get; set; }
        [JsonProperty(Order = 3)]
        public string[] an = new string[RAIDA.NODEQNTY];
        [JsonProperty(Order = 4)]
        public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018
        [JsonProperty(Order = 5)]
        public string[] aoid = new string[1];//Account or Owner ID
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CoinStackOut
    {
        public CoinStackOut(CoinStack stack)
        {
            cloudcoin = new List<CloudCoinOut>();
            foreach (CloudCoin coin in stack.cloudcoin)
            {
                cloudcoin.Add(new CloudCoinOut() { sn = coin.sn.ToString(), nn = coin.nn.ToString(), an = coin.an, aoid = coin.aoid, ed = coin.ed });
            }
        }
        [JsonProperty]
        public List<CloudCoinOut> cloudcoin { get; set; }
        public void SaveInFile(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (File.Exists(filename))
            {
                var FD = new SaveFileDialog();
                FD.InitialDirectory = fi.DirectoryName;
                FD.Title = "File Exists, Choose Another Name";
                FD.OverwritePrompt = true;
                FD.DefaultExt = "ccstack";
                FD.CreatePrompt = false;
                FD.CheckPathExists = true;
                FD.CheckFileExists = true;
                FD.AddExtension = true;
                FD.ShowDialog();
                fi = new FileInfo(FD.FileName);
            }
            Directory.CreateDirectory(fi.DirectoryName);
            using (StreamWriter sw = fi.CreateText())
            {
                string json = null;
                try
                {
                    json = JsonConvert.SerializeObject(this);
                    sw.Write(json);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("CloudStackOut.SaveInFile Serialize exception: " + ex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("CloudStackOut.SaveInFile IO exception: " + ex.Message);
                }
            }
        }
    }
}
