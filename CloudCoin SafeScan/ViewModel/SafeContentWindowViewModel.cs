/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows;

namespace CloudCoin_SafeScan
{
    class SafeContentWindowViewModel : ViewModelBase
    {
        public RelayCommand BeginFix { get; set; }
        public RelayCommand DetectFracked { get; set; }

        public class ShelfString
        {
            public string Value { get; set; }
            public int Good { get; set; }
            public int Fractioned { get; set; }
            public int Total { get; set; }
        }

        private Visibility fixButton;
        public Visibility IsFixButtonVisible
        {
            get { return fixButton; }
            set { fixButton = value; RaisePropertyChanged("IsFixButtonVisible"); }
        }

        private ShelfString[] _rows = new ShelfString[6];
        public ShelfString[] Rows
        {
            get { return _rows; }
            set { _rows = value; RaisePropertyChanged("Rows"); }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; RaisePropertyChanged("StatusText"); }
        }


        public SafeContentWindowViewModel()
        {
            BeginFix = new RelayCommand(ApplicationLogic.FixSelected);
            DetectFracked = new RelayCommand(ApplicationLogic.DetectFracked);
            Safe.Instance.SafeChanged += SafeContentChanged;
            Safe.Instance.onSafeContentChanged(new EventArgs());
            IsFixButtonVisible = Visibility.Hidden;
        }
        
        private void SafeContentChanged(object sender, EventArgs e)
        {
            Safe safe = Safe.Instance;
            StatusText = Properties.Resources.YouHave + safe.Contents.SumInStack.ToString() + " " + Properties.Resources.CCinSafe;
            if (Safe.Instance.Contents.FractionedQuantity > 0)
            {
                IsFixButtonVisible = Visibility.Visible;
            } else
            {
                IsFixButtonVisible = Visibility.Hidden;
            }
            Rows = new ShelfString[6]
            {
                new ShelfString { Value = Properties.Resources.Ones, Good = safe.Ones.GoodQuantity,
                Fractioned = safe.Ones.FractionedQuantity, Total = safe.Ones.TotalQuantity },
                new ShelfString { Value = Properties.Resources.Fives, Good = safe.Fives.GoodQuantity,
                Fractioned = safe.Fives.FractionedQuantity, Total = safe.Fives.TotalQuantity },
                new ShelfString { Value = Properties.Resources.Quarters, Good = safe.Quarters.GoodQuantity,
                Fractioned = safe.Quarters.FractionedQuantity, Total = safe.Quarters.TotalQuantity },
                new ShelfString { Value = Properties.Resources.Hundreds, Good = safe.Hundreds.GoodQuantity,
                Fractioned = safe.Hundreds.FractionedQuantity, Total = safe.Hundreds.TotalQuantity },
                new ShelfString { Value = Properties.Resources.Kiloquarters, Good = safe.KiloQuarters.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity, Total = safe.KiloQuarters.TotalQuantity },
                new ShelfString { Value = Properties.Resources.Sum,
                Good = safe.KiloQuarters.GoodQuantity*250+safe.Hundreds.GoodQuantity*100+safe.Quarters.GoodQuantity*25+safe.Fives.GoodQuantity*5+safe.Ones.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity*250+safe.Hundreds.FractionedQuantity*100+safe.Quarters.FractionedQuantity*25+safe.Fives.FractionedQuantity*5+safe.Ones.FractionedQuantity,
                Total = safe.KiloQuarters.TotalQuantity*250+safe.Hundreds.TotalQuantity*100+safe.Quarters.TotalQuantity*25+safe.Fives.TotalQuantity*5+safe.Ones.TotalQuantity }
            };
        }  
    }
}
