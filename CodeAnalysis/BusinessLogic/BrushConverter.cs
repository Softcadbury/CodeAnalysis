namespace CodeAnalysis.BusinessLogic
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// This class converts an integer to a color
    /// Red if negative, Green if positive
    /// </summary>
    public class BrushConverter : FrameworkElement, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int i;
                if (Int32.TryParse(value.ToString(), out i))
                {
                    if (i < 0)
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                    else if (i > 0)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}