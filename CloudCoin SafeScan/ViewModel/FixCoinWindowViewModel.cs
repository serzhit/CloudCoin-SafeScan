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
            set
            {
                _ns = value;
                RaisePropertyChanged("nodeStatus");
            }
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


        public CoinFixStatus(CloudCoin coin)
        {
        }
    }
}
