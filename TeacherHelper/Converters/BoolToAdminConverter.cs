using System;
using System.Globalization;
using System.Windows.Data;

namespace TeacherHelper.Converters
{
    public class BoolToAdminConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Admin" : "User";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
