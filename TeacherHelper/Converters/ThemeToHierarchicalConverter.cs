using System;
using System.Globalization;
using System.Windows.Data;
using TeacherHelper.Models.DTOs.Common;

namespace TeacherHelper.Converters
{
    public class ThemeToHierarchicalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var theme = (value as ThemeDTO);

            if (theme == null)
            {
                return null;
            }

            return new string(' ', theme.TreeDepth * 4) + theme.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
