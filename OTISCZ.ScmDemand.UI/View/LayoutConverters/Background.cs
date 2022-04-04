using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace OTISCZ.ScmDemand.UI.View.LayoutConverters
{
    public class Background : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom(SchemaColor.BackgroundPink));
            } else if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Black) {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom(SchemaColor.BackgroundBlack));
            }

            return (SolidColorBrush)(new BrushConverter().ConvertFrom(SchemaColor.BackgroundStandard));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
