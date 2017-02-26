using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _firstCheckBox, _secondCheckBox;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFirstCheckBoxChecked
        {
            get { return _firstCheckBox; }
            set { SetProperty(ref _firstCheckBox, value); }
        }

        public bool IsSecondCheckBoxChecked
        {
            get { return _secondCheckBox; }
            set { SetProperty(ref _secondCheckBox, value); }
        }

        private bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
        {
            if(!Equals(member, value))
            {
                member = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged?.Invoke(this, args);
            }
        }
    }
}
