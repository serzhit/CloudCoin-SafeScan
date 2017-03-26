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
        /// <summary>
        /// Occurs when a property is changed within an item.
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;

        public FullyObservableCollection() : base()
        { }

        public FullyObservableCollection(List<T> list) : base(list)
        {
            ObserveAll();
        }

        public FullyObservableCollection(IEnumerable<T> enumerable) : base(enumerable)
        {
            ObserveAll();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.OldItems)
                    item.PropertyChanged -= ChildPropertyChanged;
            }

            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.NewItems)
                    item.PropertyChanged += ChildPropertyChanged;
            }

            base.OnCollectionChanged(e);
        }

        protected void OnItemPropertyChanged(ItemPropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(this, e);
        }

        protected void OnItemPropertyChanged(int index, PropertyChangedEventArgs e)
        {
            OnItemPropertyChanged(new ItemPropertyChangedEventArgs(index, e));
        }

        protected override void ClearItems()
        {
            foreach (T item in Items)
                item.PropertyChanged -= ChildPropertyChanged;

            base.ClearItems();
        }

        private void ObserveAll()
        {
            foreach (T item in Items)
                item.PropertyChanged += ChildPropertyChanged;
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            T typedSender = (T)sender;
            int i = Items.IndexOf(typedSender);

            if (i < 0)
                throw new ArgumentException("Received property notification from item not in collection");

            OnItemPropertyChanged(i, e);
        }
    }

    /// <summary>
    /// Provides data for the <see cref="FullyObservableCollection{T}.ItemPropertyChanged"/> event.
    /// </summary>
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Gets the index in the collection for which the property change has occurred.
        /// </summary>
        /// <value>
        /// Index in parent collection.
        /// </value>
        public int CollectionIndex { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index in the collection of changed item.</param>
        /// <param name="name">The name of the property that changed.</param>
        public ItemPropertyChangedEventArgs(int index, string name) : base(name)
        {
            CollectionIndex = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        public ItemPropertyChangedEventArgs(int index, PropertyChangedEventArgs args) : this(index, args.PropertyName)
        { }
    }
}
