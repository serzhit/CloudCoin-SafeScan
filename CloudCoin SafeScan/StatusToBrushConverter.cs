using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CloudCoin_SafeScan
{
    public class StatusToBrushConverter : IValueConverter
    {
        public Brush PassBrush { get; set; }
        public Brush UnknownBrush { get; set; }
        public Brush FailBrush { get; set; }
        public Brush ErrorBrush { get; set; }
        public Brush FixingBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (ObservableStatus)value;
            switch (x.Status)
            {
                case CloudCoin.raidaNodeResponse.pass:
                    return PassBrush;
                case CloudCoin.raidaNodeResponse.error:
                    return ErrorBrush;
                case CloudCoin.raidaNodeResponse.fail:
                    return FailBrush;
                case CloudCoin.raidaNodeResponse.fixing:
                    return FixingBrush;
                case CloudCoin.raidaNodeResponse.unknown:
                    return UnknownBrush;
                default:
                    return UnknownBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
