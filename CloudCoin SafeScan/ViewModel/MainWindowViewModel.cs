using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.IO;
using Newtonsoft.Json;

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
            BeginScan = new RelayCommand(_BeginScan);
            ShowSafe = new RelayCommand(_ShowSafe);
            PayOut = new RelayCommand(_PayOut);

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

        private void NodeStatusChanged(object sender, RAIDA.EchoStatusChangedEventArgs e)
        {
            if (e.index < 25)
            {
                NodeStatus[e.index] = (RAIDA.Instance.EchoStatus[e.index].status == "ready") ? new ObservableBool(true) : new ObservableBool(false);
                //RaisePropertyChanged("NodeStatus");
                ToolTip[e.index] = RAIDA.Instance.NodesArray[e.index].ToString();
                //RaisePropertyChanged("ToolTip");
            }
            else
            {
                EchoCompletedText = "Done. Hover for status.";
            }
        }

        private void _BeginScan()
        {
            CloudCoinFile coinFile;
            try
            {
                coinFile = new CloudCoinFile(Utils.ChooseInputFile());
                RAIDA.Instance.Detect(coinFile.Coins);
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show("File not found: " + fnfex.Message);
            }
            catch (JsonException jsonex)
            {
                MessageBox.Show("Error in json file format: " + jsonex.Message);
            }
        }

        private void _ShowSafe()
        {

        }
        private void _PayOut()
        {

        }
    }


}
