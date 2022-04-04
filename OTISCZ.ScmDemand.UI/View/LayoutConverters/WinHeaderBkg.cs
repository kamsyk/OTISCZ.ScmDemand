using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OTISCZ.ScmDemand.UI.View.LayoutConverters
{
    public class WinHeaderBkg : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#D81B60"));
            } else if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Black) {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#6A1B9A"));
            }

            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#0D47A1"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
