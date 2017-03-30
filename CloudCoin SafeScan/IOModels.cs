using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace CloudCoin_SafeScan
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudCoinOut
    {
        [JsonProperty]
        public string sn { get; set; }
        [JsonProperty]
        public string nn { get; set; }
        [JsonProperty]
        public string[] an = new string[RAIDA.NODEQNTY];
        [JsonProperty]
        public string[] aoid = new string[1];//Account or Owner ID
        [JsonProperty]
        public string ed; //expiration in the form of Date expressed as a hex string like 97e2 Sep 2018
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
