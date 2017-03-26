using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace CloudCoin_SafeScan
{
    public class FixCoinWindowViewModel : ViewModelBase
    {

        private CloudCoin coinToFix;
        private FullyObservableCollection<ObservableStatus> _ns;
        public FullyObservableCollection<ObservableStatus> nodeStatus
        {
            get { return _ns; }
            set { _ns = value; }
        }
        private string _st;
        public string StatusText
        {
            get { return _st; }
            set { Set(ref _st, value, "StatusText"); }
        }

        public FixCoinWindowViewModel()
        {
            _ns = new FullyObservableCollection<ObservableStatus>();
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
                _ns.Add(new ObservableStatus(CloudCoin.raidaNodeResponse.unknown));
            RaisePropertyChanged("nodeStatus");
        }

        public FixCoinWindowViewModel(CloudCoin coin)
        {
            // StatusText = "blablabla";
            _ns = new FullyObservableCollection<ObservableStatus>();
            coinToFix = coin;
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
                _ns.Add( new ObservableStatus(coin.detectStatus[i]) );
            RaisePropertyChanged("nodeStatus");
        }

        public void SetStatus(string message)
        {
            StatusText = message;
        }
    }

    public class FixStackViewModel : ViewModelBase
    {
        private CoinStack stack;
        private FullyObservableCollection<CoinFixStatus> fixingCoinsStatuses;
        public FullyObservableCollection<CoinFixStatus> FixingCoins
        {
            get { return fixingCoinsStatuses; }
            set { fixingCoinsStatuses = value; }
        }

        public FixStackViewModel()
        {
            fixingCoinsStatuses = new FullyObservableCollection<CoinFixStatus>();
        }

        public FixStackViewModel(CoinStack stack)
        {
            this.stack = stack;
            fixingCoinsStatuses = new FullyObservableCollection<CoinFixStatus>();
            foreach(CloudCoin coin in stack)
            {
                fixingCoinsStatuses.Add(new CoinFixStatus(coin));
            }
        }
        public FixStackViewModel(List<CloudCoin> list)
        {
            fixingCoinsStatuses = new FullyObservableCollection<CoinFixStatus>();
            foreach (CloudCoin coin in list)
            {
                fixingCoinsStatuses.Add(new CoinFixStatus(coin));
            }
        }
    }

    public class CoinFixStatus : ViewModelBase
    {
        public int serial { get; set; }
        public CloudCoin.raidaNodeResponse[] coinStatus { get; set; }
        public string status { get; set; }
        public CloudCoin.Denomination Denomination { get; set; }

        public CoinFixStatus()
        {
            for (int i=0; i<coinStatus.Length; i++)
                coinStatus[i] = CloudCoin.raidaNodeResponse.unknown;
            status = "Not set";
            Denomination = CloudCoin.Denomination.Unknown;
        }

        public CoinFixStatus(CloudCoin coin)
        {
            serial = coin.sn;
            coinStatus = coin.detectStatus;
            status = "Initialized";
        }

    }
}
