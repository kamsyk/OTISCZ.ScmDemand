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
    public class StandardBkgImg : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            //if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
            //    BitmapImage myBitmapImage = new BitmapImage();

            //    myBitmapImage.BeginInit();
            //    myBitmapImage.UriSource = new Uri(@"/OTISCZ.ScmDemand.UI;component/Images/Flower.png");
            //    myBitmapImage.DecodePixelWidth = 200;
            //    myBitmapImage.EndInit();

            //    return myBitmapImage;
            //} else if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Black) {
            //    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#D50000"));
            //}

            //return (SolidColorBrush)(new BrushConverter().ConvertFrom("#0D47A1"));

            if (MainWindow.ScmLayoutStyle == MainWindow.LayoutStyle.Pink) {
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/OTISCZ.ScmDemand.UI;component/Images/Flower.png");
                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();

                return myBitmapImage;
            } else {
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/OTISCZ.ScmDemand.UI;component/Images/Empty.png");
                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();

                return myBitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
