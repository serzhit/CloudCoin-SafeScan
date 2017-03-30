using System;
using System.Windows;

//TODO: Make SafeContentViewModel class

namespace CloudCoin_SafeScan
{
    /// <summary>
    /// Interaction logic for SafeContents.xaml
    /// </summary>
    public partial class SafeContentWindow : Window
    {
        public SafeContentWindow()
        {
            InitializeComponent();
            Owner = MainWindow.Instance;
            Safe safe = Safe.Instance;
            totalTextBox.Text = safe.Contents.SumInStack.ToString() + " CC in Safe";
            SafeView.ItemsSource = new SafeContentWindowViewModel[6]
            {
                new SafeContentWindowViewModel { Value = "Ones", Good = safe.Ones.GoodQuantity,
                Fractioned = safe.Ones.FractionedQuantity, Counterfeited = safe.Ones.CounterfeitedQuantity, Total = safe.Ones.TotalQuantity },
                new SafeContentWindowViewModel { Value = "Fives", Good = safe.Fives.GoodQuantity,
                Fractioned = safe.Fives.FractionedQuantity, Counterfeited = safe.Fives.CounterfeitedQuantity, Total = safe.Fives.TotalQuantity },
                new SafeContentWindowViewModel { Value = "Quarters", Good = safe.Quarters.GoodQuantity,
                Fractioned = safe.Quarters.FractionedQuantity, Counterfeited = safe.Quarters.CounterfeitedQuantity, Total = safe.Quarters.TotalQuantity },
                new SafeContentWindowViewModel { Value = "Hundreds", Good = safe.Hundreds.GoodQuantity,
                Fractioned = safe.Hundreds.FractionedQuantity, Counterfeited = safe.Hundreds.CounterfeitedQuantity, Total = safe.Hundreds.TotalQuantity },
                new SafeContentWindowViewModel { Value = "250s", Good = safe.KiloQuarters.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity, Counterfeited = safe.KiloQuarters.CounterfeitedQuantity, Total = safe.KiloQuarters.TotalQuantity },
                new SafeContentWindowViewModel { Value = "Sum:",
                Good = safe.KiloQuarters.GoodQuantity*250+safe.Hundreds.GoodQuantity*100+safe.Quarters.GoodQuantity*25+safe.Fives.GoodQuantity*5+safe.Ones.GoodQuantity,
                Fractioned = safe.KiloQuarters.FractionedQuantity*250+safe.Hundreds.FractionedQuantity*100+safe.Quarters.FractionedQuantity*25+safe.Fives.FractionedQuantity*5+safe.Ones.FractionedQuantity,
                Counterfeited = safe.KiloQuarters.CounterfeitedQuantity*250+safe.Hundreds.CounterfeitedQuantity*100+safe.Quarters.CounterfeitedQuantity*25+safe.Fives.CounterfeitedQuantity*5+safe.Ones.CounterfeitedQuantity,
                Total = safe.KiloQuarters.TotalQuantity*250+safe.Hundreds.TotalQuantity*100+safe.Quarters.TotalQuantity*25+safe.Fives.TotalQuantity*5+safe.Ones.TotalQuantity }
            };

        }

        private void onCloseButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void onFixButton(object sender, RoutedEventArgs e)
        {
            Safe.Instance?.onBeforeFixStart(EventArgs.Empty);
        }
    }
}
