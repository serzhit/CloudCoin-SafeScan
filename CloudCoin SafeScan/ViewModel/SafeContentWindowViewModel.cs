using GalaSoft.MvvmLight;

namespace CloudCoin_SafeScan
{
    class SafeContentWindowViewModel : ViewModelBase
    {
        public struct ShelfString
        {
            public string Value;
            public int Good;
            public int Fractioned;
            public int Counterfeited;
            public int Total;
        }

        private ShelfString[] _rows = new ShelfString[6];
        public ShelfString[] Rows
        {
            get { return _rows; }
            set { _rows = value; RaisePropertyChanged("Row"); }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; RaisePropertyChanged("StatusText"); }
        }


        public SafeContentWindowViewModel()
        {
            Safe safe = Safe.Instance;
            StatusText = safe.Contents.SumInStack.ToString() + " CC in Safe";
            Rows = new ShelfString[6]
            {
                new ShelfString { Value = "Ones", Good = safe.Ones.GoodQuantity,
                Fractioned = safe.Ones.FractionedQuantity, Counterfeited = safe.Ones.CounterfeitedQuantity, Total = safe.Ones.TotalQuantity },
                new ShelfString { Value = "Fives", Good = safe.Fives.GoodQuantity,
                Fractioned = safe.Fives.FractionedQuantity, Counterfeited = safe.Fives.CounterfeitedQuantity, Total = safe.Fives.TotalQuantity },
                new ShelfString { Value = "Quarters", Good = safe.Quarters.GoodQuantity,
                Fractioned = safe.Quarters.FractionedQuantity, Counterfeited = safe.Quarters.CounterfeitedQuantity, Total = safe.Quarters.TotalQuantity },
                new ShelfString { Value = "Hundreds", Good = safe.Hundreds.GoodQuantity,
                Fractioned = safe.Hundreds.FractionedQuantity, Counterfeited = safe.Hundreds.CounterfeitedQuantity, Total = safe.Hundreds.TotalQuantity },
                new ShelfString { Value = "250s", Good = safe.KiloQuarters.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity, Counterfeited = safe.KiloQuarters.CounterfeitedQuantity, Total = safe.KiloQuarters.TotalQuantity },
                new ShelfString { Value = "Sum:",
                Good = safe.KiloQuarters.GoodQuantity*250+safe.Hundreds.GoodQuantity*100+safe.Quarters.GoodQuantity*25+safe.Fives.GoodQuantity*5+safe.Ones.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity*250+safe.Hundreds.FractionedQuantity*100+safe.Quarters.FractionedQuantity*25+safe.Fives.FractionedQuantity*5+safe.Ones.FractionedQuantity,
                Counterfeited = safe.KiloQuarters.CounterfeitedQuantity*250+safe.Hundreds.CounterfeitedQuantity*100+safe.Quarters.CounterfeitedQuantity*25+safe.Fives.CounterfeitedQuantity*5+safe.Ones.CounterfeitedQuantity,
                Total = safe.KiloQuarters.TotalQuantity*250+safe.Hundreds.TotalQuantity*100+safe.Quarters.TotalQuantity*25+safe.Fives.TotalQuantity*5+safe.Ones.TotalQuantity }
            };
        }  
    }
}
