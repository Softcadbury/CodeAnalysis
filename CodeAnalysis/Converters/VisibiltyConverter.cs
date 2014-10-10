namespace CodeAnalysis.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// This class converts a boolean to a visibility
    /// </summary>
    public class VisibiltyConverter : FrameworkElement, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var isVisible = (bool)value;

                if (isVisible)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}