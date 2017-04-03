using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CloudCoin_SafeScan
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand BeginScan { get; set; }
        public RelayCommand ShowSafe { get; set; }
        public RelayCommand PayOut { get; set; }

        private string _echoCompletedText;
        public string EchoCompletedText
        {
            get { return _echoCompletedText; }
            set
            {
                _echoCompletedText = value;
                RaisePropertyChanged("EchoCompletedText");
            }
        }

        private FullyObservableCollection<ObservableBool> _nodeStatus = new FullyObservableCollection<ObservableBool>();
        public FullyObservableCollection<ObservableBool> NodeStatus
        {
            get { return _nodeStatus; }
            set { _nodeStatus = value;  RaisePropertyChanged("NodeStatus"); }
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

        public MainWindowViewModel()
        {
            BeginScan = new RelayCommand(ApplicationLogic.ScanSelected);
            ShowSafe = new RelayCommand(ApplicationLogic.SafeSelected);
            PayOut = new RelayCommand(ApplicationLogic.PaySelected);

            _echoCompletedText = "Echoing RAIDA nodes...";
            for(int i=0;i<RAIDA.NODEQNTY;i++)
            {
                _toolTip.Add(new ObservableString("Node hasn't respond yet..."));
            }
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
            {
                _nodeStatus.Add(new ObservableBool(false));
            }
            RAIDA.Instance.EchoStatusChanged += NodeStatusChanged;
        }

        private void NodeStatusChanged(object sender, EchoStatusChangedEventArgs e)
        {
            if (e.index < 25)
            {
                NodeStatus[e.index] = (RAIDA.Instance.EchoStatus[e.index].status == "ready") ? new ObservableBool(true) : new ObservableBool(false);
                ToolTip[e.index] = RAIDA.Instance.NodesArray[e.index].ToString();
            }
            else
            {
                EchoCompletedText = "Done. Hover for status.";
            }
        }
    }
}
