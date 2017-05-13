/***
 * This software is distributed under MIT License
 * Cloudcoin Consortium, Sergey Gitinsky (c)2017
 * All rights reserved
 */
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System.Collections.Generic;
using System.Threading;

namespace CloudCoin_SafeScan
{
    public class FixCoinViewModel : ViewModelBase
    {
        private int _sn;
        public int SN
        {
            get { return _sn; }
            set { Set(ref _sn, value, "SN"); }
        }

        private string _denom;
        public string Denomination
        {
            get { return _denom; }
            set { Set(ref _denom, value, "Denomination"); }
        }

        private FullyObservableCollection<ObservableStatus> _ns;
        public FullyObservableCollection<ObservableStatus> NodeStatus
        {
            get { return _ns; }
            set
            {
                _ns = value;
                RaisePropertyChanged("NodeStatus");
            }
        }
        private string _st;
        public string StatusText
        {
            get { return _st; }
            set { Set(ref _st, value, "StatusText"); }
        }

        public FixCoinViewModel()
        {
            NodeStatus = new FullyObservableCollection<ObservableStatus>();
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
                NodeStatus.Add(new ObservableStatus(CloudCoin.raidaNodeResponse.unknown));
            SN = 12345655;
            Denomination = "100";
            StatusText = "Fixing node 12...";

        }
        public FixCoinViewModel(CloudCoin coin)
        {
            NodeStatus = new FullyObservableCollection<ObservableStatus>();
            for (int i = 0; i < RAIDA.NODEQNTY; i++)
                NodeStatus.Add(new ObservableStatus(coin.detectStatus[i]));
            SN = coin.sn;
            Denomination = coin.denomination.ToString();
            StatusText = "Ready to fix...";
        }
    }

    public class FixStackViewModel : ViewModelBase
    {
        private IEnumerable<CloudCoin> FrackedCoins;
        private FullyObservableCollection<FixCoinViewModel> fixingCoinsStatuses;
        public FullyObservableCollection<FixCoinViewModel> FixingCoins
        {
            get { return fixingCoinsStatuses; }
            set { fixingCoinsStatuses = value; RaisePropertyChanged("FixingCoins"); }
        }

        public FixStackViewModel()
        {
            RAIDA.Instance.CoinFixStarted += CoinFixStarted;
            RAIDA.Instance.CoinFixProcessing += CoinFixProcessing;
            RAIDA.Instance.CoinFixFinished += CoinFixFinished;
            FrackedCoins = Safe.Instance.FrackedCoinsList;
            FixingCoins = new FullyObservableCollection<FixCoinViewModel>();
            foreach(CloudCoin coin in FrackedCoins)
            {
                FixCoinViewModel CoinStatus = new FixCoinViewModel(coin);
                FixingCoins.Add(CoinStatus);
            }
        }

        private void CoinFixStarted(object sender, CoinFixStartedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(()=> 
            {
                FixCoinViewModel coinBeingFixed;
                coinBeingFixed = FixingCoins[e.coinindex];
                coinBeingFixed.StatusText = "Fixing key on node " + e.NodeNumber + "...";
                coinBeingFixed.NodeStatus[e.NodeNumber] = new ObservableStatus(CloudCoin.raidaNodeResponse.fixing);
            });
            
        }

        private void CoinFixProcessing(object sender, CoinFixProcessingEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                FixCoinViewModel coinBeingFixed;
                coinBeingFixed = FixingCoins[e.coinindex];
                coinBeingFixed.StatusText = "Processing Key on node " + e.NodeNumber + ", corner " + e.corner;
            });
        }

        private void CoinFixFinished(object sender, CoinFixFinishedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                FixCoinViewModel coinBeingFixed;
                coinBeingFixed = FixingCoins[e.coinindex];
                coinBeingFixed.StatusText = "Key on node " + e.NodeNumber + " was " + (e.result == CloudCoin.raidaNodeResponse.pass ? "" : "not") + " fixed!";
                coinBeingFixed.NodeStatus[e.NodeNumber] = new ObservableStatus(e.result);
/*                Thread.Sleep(1000);
                if(e.result == CloudCoin.raidaNodeResponse.pass)
                {
                    FixingCoins.RemoveAt(e.coinindex);
                } */
            });
        }
    }
}
