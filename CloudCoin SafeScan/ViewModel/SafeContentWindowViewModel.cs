using GalaSoft.MvvmLight;
using System;

namespace CloudCoin_SafeScan
{
    class SafeContentWindowViewModel : ViewModelBase
    {
        public class ShelfString
        {
            public string Value { get; set; }
            public int Good { get; set; }
            public int Fractioned { get; set; }
            public int Total { get; set; }
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
            Safe.Instance.SafeChanged += SafeContentChanged;
            Safe.Instance.onSafeContentChanged(new EventArgs());
        }
        
        private void SafeContentChanged(object sender, EventArgs e)
        {
            Safe safe = Safe.Instance;
            StatusText = safe.Contents.SumInStack.ToString() + " CC in Safe";
            Rows = new ShelfString[6]
            {
                new ShelfString { Value = "Ones", Good = safe.Ones.GoodQuantity,
                Fractioned = safe.Ones.FractionedQuantity, Total = safe.Ones.TotalQuantity },
                new ShelfString { Value = "Fives", Good = safe.Fives.GoodQuantity,
                Fractioned = safe.Fives.FractionedQuantity, Total = safe.Fives.TotalQuantity },
                new ShelfString { Value = "Quarters", Good = safe.Quarters.GoodQuantity,
                Fractioned = safe.Quarters.FractionedQuantity, Total = safe.Quarters.TotalQuantity },
                new ShelfString { Value = "Hundreds", Good = safe.Hundreds.GoodQuantity,
                Fractioned = safe.Hundreds.FractionedQuantity, Total = safe.Hundreds.TotalQuantity },
                new ShelfString { Value = "250s", Good = safe.KiloQuarters.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity, Total = safe.KiloQuarters.TotalQuantity },
                new ShelfString { Value = "Sum:",
                Good = safe.KiloQuarters.GoodQuantity*250+safe.Hundreds.GoodQuantity*100+safe.Quarters.GoodQuantity*25+safe.Fives.GoodQuantity*5+safe.Ones.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity*250+safe.Hundreds.FractionedQuantity*100+safe.Quarters.FractionedQuantity*25+safe.Fives.FractionedQuantity*5+safe.Ones.FractionedQuantity,
                Total = safe.KiloQuarters.TotalQuantity*250+safe.Hundreds.TotalQuantity*100+safe.Quarters.TotalQuantity*25+safe.Fives.TotalQuantity*5+safe.Ones.TotalQuantity }
            };
        }  
    }
}
