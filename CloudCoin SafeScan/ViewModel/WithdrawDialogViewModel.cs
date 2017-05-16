/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using GalaSoft.MvvmLight;
using System;

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
            Safe.Instance.SafeChanged += SafeContentChanged;
            SumInSafe = Safe.Instance.Contents.SumInStack;
        }

        private void SafeContentChanged(object sender, EventArgs e)
        {
            SumInSafe = Safe.Instance.Contents.SumInStack;
        }
    }
}
