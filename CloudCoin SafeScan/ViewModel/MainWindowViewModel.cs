using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoin_SafeScan
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand BeginScan { get; set; }
        public RelayCommand ShowSafe { get; set; }
        public RelayCommand PayOut { get; set; }

        public string EchoCompletedText { get; set; }

        public RAIDA.EchoResponse[] RAIDAEchoResponse = new RAIDA.EchoResponse[RAIDA.NODEQNTY];

        public MainWindowViewModel()
        {
            BeginScan = new RelayCommand(_BeginScan);
            ShowSafe = new RelayCommand(_ShowSafe);
            PayOut = new RelayCommand(_PayOut);
        }

        private void _BeginScan()
        {
            MessageBox.Show("MVVM works OK!");
        }
        private void _ShowSafe()
        {

        }
        private void _PayOut()
        {

        }
    }


}
