using System.IO;
using System.Text;
using System.Windows;
using CryptSharp;

namespace CloudCoin_SafeScan
{
    internal static class UserInteract
    {
        internal static string SetPassword()
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

        internal static string CheckPassword(FileInfo fi)
        {
            using (var fs = fi.Open(FileMode.Open))
            {
                byte[] buffer = new byte[60];
                byte[] passbytes = { 1, 2, 3, 4 }; //bogus data just to initialize
                fs.Read(buffer, 0, 60);
                string cryptPassFromFile = new string(Encoding.UTF8.GetChars(buffer));
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
                            fs.Close();
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
                        fs.Close();
                        break;
                    }
                }
            }
            return "error";
        }
    }
}
