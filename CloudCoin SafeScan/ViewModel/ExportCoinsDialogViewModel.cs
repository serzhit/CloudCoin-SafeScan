using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    class ExportCoinsDialogViewModel : ViewModelBase
    {
        private int _sumInSafe;
        public int SumInSafe
        {
            get { return _sumInSafe; }
            set
            {
                _sumInSafe = value;
                RaisePropertyChanged("SumInSafe");
            }
        }

        private string _welcomeText;
        public string WelcomeText
        {
            get { return _welcomeText; }
            set
            {
                _welcomeText = value;
                RaisePropertyChanged("WelcomeText");
            }
        }

        public ExportCoinsDialogViewModel()
        {
            WelcomeText = "Enter Sum you want to withdraw.\n" +
                "App will form file with stack of coins\nwith nearest possible amount of coins.";
            SumInSafe = Safe.Instance.Contents.SumInStack;
        }
    }
}
