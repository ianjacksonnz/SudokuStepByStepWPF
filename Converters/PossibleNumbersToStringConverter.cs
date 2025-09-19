using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SudokuStepByStep.Converters
{
    public class PossibleNumbersToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Accept any IEnumerable and filter for ints
            if (value is IEnumerable enumerable)
            {
                var numbers = enumerable.Cast<object>()
                                       .OfType<int>()
                                       .ToList();

                if (numbers.Count > 0)
                {
                    return string.Join(" ", numbers);
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
