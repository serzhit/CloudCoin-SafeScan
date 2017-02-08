using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Security.AccessControl;
using CryptSharp;
using Newtonsoft.Json;

namespace CloudCoin_SafeScan
{
    public class Safe
    {
        public string password;
        public string safeFilePath;
        public FileInfo safeFileInfo;
        private string cryptedPass;
        private CoinStack Contents;
        
        public Safe()
        {
            safeFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                "\\Cloudcoin\\" + Environment.UserName + ".safe";
            safeFileInfo = new FileInfo(safeFilePath);
            if(!safeFileInfo.Exists)
            {
                var passwordWindow = new SetPassword();
                passwordWindow.ShowDialog();
                password = passwordWindow.Password.Password;
                byte[] passbytes = Encoding.UTF8.GetBytes(password);
                cryptedPass = Crypter.Blowfish.Crypt(passbytes);
                byte[] cryptedpassbytes = Encoding.UTF8.GetBytes(cryptedPass);
//                var dirsecurity = new DirectorySecurity();
//                dirsecurity.SetAccessRule()
                Directory.CreateDirectory(safeFileInfo.DirectoryName);
                Contents = new CoinStack();
                var json = JsonConvert.SerializeObject(Contents);
                var cryptedjson = Convert.Encrypt(json, password, cryptedpassbytes.Take(16).ToArray());
                using (var fs = safeFileInfo.Create())
                {
                    fs.Write(cryptedpassbytes,0,60);
                    fs.Write(cryptedjson, 0, cryptedjson.Length);
                    fs.Close();
                }
                
            }
            else
            {
                using (var fs = safeFileInfo.Open(FileMode.Open))
                {
                    byte[] buffer = new byte[60];
                    fs.Read(buffer, 0, 60);
                    string cryptedPassSafe = new string(Encoding.UTF8.GetChars(buffer));
                    var enterPassword = new EnterPassword();
                    enterPassword.ShowDialog();
                    var testpassword = enterPassword.passwordBox.Password;
                    byte[] passbytes = Encoding.UTF8.GetBytes(testpassword);
                    while (!Crypter.CheckPassword(passbytes, cryptedPassSafe))
                    {
                        MessageBox.Show("Wrong password from safe.\nTry again.");
                        enterPassword.ShowDialog();
                        testpassword = enterPassword.passwordBox.Password;
                        passbytes = Encoding.UTF8.GetBytes(testpassword);
                    }
                    password = testpassword;
                    cryptedPass = cryptedPassSafe;
                    byte[] cryptedjson = new byte[(int)(fs.Length - 60)];
                    try
                    {
                        var numbytestoread = fs.Length - 60;
                        int numbytesread = 0;
                        while (true)
                        {
                            var n = fs.Read(cryptedjson, numbytesread, (int)numbytestoread);
                            if (n == 0) break;
                            numbytesread += n;
                            numbytestoread -= n;
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("IO Exception: " + ex.Message);
                    }
                    string json = Convert.Decrypt(cryptedjson, password, Encoding.UTF8.GetBytes(cryptedPass).Take(16).ToArray());
                    try
                    {
                        Contents = JsonConvert.DeserializeObject<CoinStack>(json);
                    }
                    catch (JsonException ex)
                    {
                        MessageBox.Show("JSON Exception: " + ex.Message);
                    }
                    fs.Close();
                }
            }
        }

        public void save(CoinStack stack)
        {
            safeFileInfo.Refresh();
            if (!safeFileInfo.Exists)
                return;
            using (var fs = safeFileInfo.Open(FileMode.Open))
            {
                Contents.Add(stack);
                Contents.cloudcoin.Sort(new CloudCoin.CoinComparer());
                string jsonstring = "";
                try
                {
                    jsonstring = JsonConvert.SerializeObject(Contents);
                }
                catch (JsonException e)
                {
                    MessageBox.Show("Exception: " + e.Message);
                }
                var cryptedjsonbytes = Convert.Encrypt(jsonstring, password, Encoding.UTF8.GetBytes(cryptedPass).Take(16).ToArray());
                fs.Write(Encoding.UTF8.GetBytes(cryptedPass), 0, 60);
                fs.Write(cryptedjsonbytes, 0, cryptedjsonbytes.Length);
                fs.Close();
            }
        }

        public void Show()
        {

        }
    }
}
