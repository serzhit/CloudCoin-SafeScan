using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    class WithdrawDialogViewModel : ViewModelBase
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

        public WithdrawDialogViewModel()
        {
            SumInSafe = Safe.Instance.Contents.SumInStack;
        }

    }
}
