﻿namespace CodeAnalysis.BusinessLogic
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// This class convert an int to a color
    /// </summary>
    public class BrushConverter : FrameworkElement, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int i;
                Int32.TryParse(value.ToString(), out i);

                if (i < 0)
                {
                    return new SolidColorBrush(Colors.Red);
                }

                if (i > 0)
                {
                    return new SolidColorBrush(Colors.Green);
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