using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using GalaSoft.MvvmLight;


namespace CloudCoin_SafeScan
{
    public class ObservableStatus : ObservableObject
    {
        private CloudCoin.raidaNodeResponse _status;
        public CloudCoin.raidaNodeResponse Status
        {
            get { return _status; }
            set { Set("Status", ref _status, value); }
        }
        public ObservableStatus()
        {
            _status = CloudCoin.raidaNodeResponse.unknown;
        }
        public ObservableStatus(CloudCoin.raidaNodeResponse st)
        {
            _status = st;
        }
    }

    public class ObservableBool : ObservableObject
    {
        private CloudCoin.raidaNodeResponse _status;
        private bool isTrue;
        public ObservableBool()
        {
            isTrue = false;
        }
        public ObservableBool(CloudCoin.raidaNodeResponse st)
        {
            if (st == CloudCoin.raidaNodeResponse.pass)
                isTrue = true;
            else
                isTrue = false;
        }
        public ObservableBool(bool b)
        {
            isTrue = b;
        }
        public static implicit operator bool(ObservableBool d)  // implicit ObservableStatus to bool conversion operator
        {
            return d.isTrue;   // implicit conversion
        }
        public static implicit operator ObservableBool(bool b)  // implicit ObservableStatus to bool conversion operator
        {
            return new ObservableBool(b);   // implicit conversion
        }
    }

    public class ObservableString : ObservableObject
    {
        private string value;
        public ObservableString()
        {
            value = null;
        }
        public ObservableString(string s)
        {
            value = s;
        }
        public static implicit operator string(ObservableString d)  // implicit ObservableStatus to bool conversion operator
        {
            return d.value;   // implicit conversion
        }
        public static implicit operator ObservableString(string s)  // implicit ObservableStatus to bool conversion operator
        {
            return new ObservableString(s);   // implicit conversion
        }
        public override string ToString()
        {
            return value;
        }
    }
    public class ObservableRaidaNodeResponse : ObservableObject
    {
        private CloudCoin.raidaNodeResponse _status;
        private int raidaNodenumber;
        private string _message;
        private int serialNumber;
        private DateTime dateTime;
        private TimeSpan responseTime;

        public int server
        {
            get { return raidaNodenumber; }
            set { Set("server", ref raidaNodenumber, value); }
        }
        public string status
        {
            get { return _status.ToString(); }
            set
            {
                if (!Enum.TryParse(value, out _status))
                    throw new Exception("Trying to set value out of range");
                RaisePropertyChanged("status");
            }
        }
        public int sn
        {
            get { return serialNumber; }
            set
            {
                Set("sn", ref serialNumber, value);
            }
        }
        public string message
        {
            get { return _message; }
            set { Set("message", ref _message, value); }
        }
        public DateTime time
        {
            get { return dateTime; }
            set { Set("time", ref dateTime, value); }
        }
    }

    public class FullyObservableCollection<T> : ObservableCollection<T>
            where T : INotifyPropertyChanged
    {
        public FullyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public FullyObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
