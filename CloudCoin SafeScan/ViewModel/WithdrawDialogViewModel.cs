/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using GalaSoft.MvvmLight;

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
