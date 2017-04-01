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

        public SafeContentWindowViewModel SafeContent
        {
            get { return ServiceLocator.Current.GetInstance<SafeContentWindowViewModel>(); }
        }

        public SelectOutStackWindowViewModel SelectOutStack
        {
            get { return ServiceLocator.Current.GetInstance<SelectOutStackWindowViewModel>(); }
        }

        public FixCoinWindowViewModel FixCoin
        {
            get { return ServiceLocator.Current.GetInstance<FixCoinWindowViewModel>(); }
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<SafeContentWindowViewModel>();
            SimpleIoc.Default.Register<SelectOutStackWindowViewModel>();
            SimpleIoc.Default.Register<FixCoinWindowViewModel>();

        }

        public static void Cleanup()
        {
        }
    }
}