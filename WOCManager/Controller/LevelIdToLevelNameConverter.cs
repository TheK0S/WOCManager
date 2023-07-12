using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WOCManager.Controller
{
    internal class LevelIdToLevelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                int level = (int)value;

                if (level == 1) return "Beginner/A1";
                if (level == 2) return "Pre-Intermediate/A2";
                if (level == 3) return "Intermediate/B1";
                if (level == 4) return "Upper-Intermediate/B2";
                if (level == 5) return "Advanced/C1";
                if (level == 6) return "Proficiency/C2";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
