using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace CloudCoin_SafeScan
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }

        public CheckCoinsWindowViewModel CheckCoins
        {
            get { return ServiceLocator.Current.GetInstance<CheckCoinsWindowViewModel>(); }
        }
        public SafeContentWindowViewModel SafeContent
        {
            get { return ServiceLocator.Current.GetInstance<SafeContentWindowViewModel>(); }
        }

        public SelectOutStackWindowViewModel SelectOutStack
        {
            get { return ServiceLocator.Current.GetInstance<SelectOutStackWindowViewModel>(); }
        }

        public WithdrawDialogViewModel Withdraw
        {
            get { return ServiceLocator.Current.GetInstance<WithdrawDialogViewModel>(); }
        }

        public FixStackViewModel FixStack
        {
            get { return ServiceLocator.Current.GetInstance<FixStackViewModel>(); }
        }
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<SafeContentWindowViewModel>();
            SimpleIoc.Default.Register<SelectOutStackWindowViewModel>();
            SimpleIoc.Default.Register<FixStackViewModel>();
            SimpleIoc.Default.Register<CheckCoinsWindowViewModel>();
            SimpleIoc.Default.Register<WithdrawDialogViewModel>();

        }

        public static void Cleanup()
        {
        }
    }
}