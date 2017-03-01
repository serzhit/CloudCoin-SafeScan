using GalaSoft.MvvmLight;

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
}
