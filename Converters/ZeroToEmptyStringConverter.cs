using System;
using System.Globalization;
using System.Windows.Data;

namespace SudokuStepByStep.Converters;

public class ZeroToEmptyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && intValue == 0)
            return string.Empty;
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value as string))
            return 0;
        if (int.TryParse(value as string, out int result))
            return result;
        return 0;
    }
}