using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace CloudCoin_SafeScan
{
    class CheckCoinsWindowViewModel : ViewModelBase
    {
        int Count; //number of coins in stack of this instance
        public class Coin4Display : ObservableObject
        {
            public int Serial { get; set; }
            public int Value { get; set; }
            public bool Check { get; set; }
            public string Comment { get; set; }
        }

        private FullyObservableCollection<Coin4Display> _checkLog;
        public FullyObservableCollection<Coin4Display> CheckLog
        {
            get { return _checkLog; }
            set
            {
                _checkLog = value;
                RaisePropertyChanged("CheckLog");
            }
        }

        private string _percentDone;
        public string PercentDone
        {
            get { return _percentDone; }
            set
            {
                _percentDone = value;
                RaisePropertyChanged("PercentDone");
            }
        }

        private string _textOnMap;
        public string TextOnMap
        {
            get { return _textOnMap; }
            set
            {
                _textOnMap = value;
                RaisePropertyChanged("TextOnMap");
            }
        }

        private double _progressBar;
        public double ProgressBar
        {
            get { return _progressBar; }
            set
            {
                if (value >= 0 && value <= TotalPercent)
                {
                    _progressBar = value;
                    RaisePropertyChanged("ProgressBar");
                }
            }
        }
        private int _totalPercent;
        public int TotalPercent
        {
            get { return _totalPercent; }
            set
            {
                    _totalPercent = value;
            }
        }

        private FullyObservableCollection<ObservableBool> _nodeStatus = new FullyObservableCollection<ObservableBool>();
        public FullyObservableCollection<ObservableBool> NodeStatus
        {
            get { return _nodeStatus; }
            set { _nodeStatus = value; RaisePropertyChanged("NodeStatus"); }
        }

        private FullyObservableCollection<ObservableString> _toolTip = new FullyObservableCollection<ObservableString>();
        public FullyObservableCollection<ObservableString> ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                RaisePropertyChanged("ToolTip");
            }
        }

        public CheckCoinsWindowViewModel(CoinStack stack)
        {
            Count = stack.cloudcoin.Count;
            TotalPercent = Count * 100;
            TextOnMap = "Authenticating coins...";
            ProgressBar = 0;
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                NodeStatus.Add(false);
            }
            CheckLog = new FullyObservableCollection<Coin4Display>();
            PercentDone = "0%";
            RAIDA.Instance.DetectCoinCompleted += CoinScanCompleted;
            RAIDA.Instance.StackScanCompleted += StackScanCompleted;

        }

        private void CoinScanCompleted(object o, DetectCoinCompletedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                for (int i = 0; i < RAIDA.NODEQNTY; i++)
                {
                    NodeStatus[i] = (e.coin.detectStatus[i].status == "pass");
                }
                CheckLog.Add(new Coin4Display()
                {
                    Serial = e.coin.sn,
                    Value = Utils.Denomination2Int(e.coin.denomination),
                    Check = e.coin.isPassed,
                    Comment = e.coin.Verdict.ToString() + ": " +
                        e.coin.percentOfRAIDAPass + "% good. Checked in " + e.sw.ElapsedMilliseconds + " ms."
                });
                ProgressBar += 100;
                PercentDone = (ProgressBar / Count).ToString() + "%";
            });
        }

        private void StackScanCompleted(object o, StackScanCompletedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TextOnMap = e.stack.cloudcoin.Count.ToString() + " coins scanned.";
            });
        }

    }
}
