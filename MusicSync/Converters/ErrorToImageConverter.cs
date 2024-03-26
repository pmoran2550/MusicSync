using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MusicSync.Converters
{
    public class ErrorToImageConverter : IValueConverter
    {
        Uri uriGoodSource = new Uri("pack://application:,,,/Resources/Good20c.png");
        Uri uriErrorSource = new Uri("pack://application:,,,/Resources/error20.png");
        Uri uriWarningSource = new Uri("pack://application:,,,/Resources/warning20.png");

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Uri uriToUse = uriErrorSource;
            Uri uriToUse;

            if (value.Equals(ErrorStatus.No_Error))
            {
                uriToUse = uriGoodSource;
            }
            else if (value.Equals(ErrorStatus.Duplicate))
            {
                uriToUse = uriWarningSource;
            }
            else
            {
                uriToUse = uriErrorSource;
            }

            BitmapImage newBitmap = new BitmapImage(uriToUse);

            return newBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
