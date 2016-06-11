using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Blotter.Converters
{
    [ValueConversion(typeof(bool), typeof(DataGridRowDetailsVisibilityMode))]
    public class BoolToRowDetailsVisibility : IValueConverter
    {
        private readonly DataGridRowDetailsVisibilityMode trueValue = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        private readonly DataGridRowDetailsVisibilityMode falseValue = DataGridRowDetailsVisibilityMode.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool))
            {
                return null;
            }
            return (bool)value ? trueValue : falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is DataGridRowDetailsVisibilityMode))
            {
                return null;
            }

            if ((DataGridRowDetailsVisibilityMode)value == trueValue) return true;
            if ((DataGridRowDetailsVisibilityMode)value == falseValue) return false;

            return null;
        }
    }
}
