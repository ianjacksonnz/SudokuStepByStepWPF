using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SudokuStepByStep.Converters
{
    public class SudokuBorderThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Defensive checks for binding values
            int col = (values[0] is int c) ? c : 0;
            int row = (values[1] is int r) ? r : 0;

            double left = col % 3 == 0 ? 2 : 0.5;
            double top = row % 3 == 0 ? 2 : 0.5;
            double right = (col + 1) % 3 == 0 ? 2 : 0.5;
            double bottom = (row + 1) % 3 == 0 ? 2 : 0.5;

            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}