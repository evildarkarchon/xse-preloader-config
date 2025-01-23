using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace xse_preloader_config;

/// A value converter that transforms a string into a boolean value.
/// This converter is commonly used in data binding scenarios, where
/// a string needs to be evaluated as a boolean.
public class StringToBooleanConverter : IValueConverter
{
    /// Converts a string to a boolean value. If the string is not null or empty, it returns true; otherwise, false.
    /// <param name="value">The input value to be converted, expected to be a string.</param>
    /// <param name="targetType">The target type for the conversion. Not used in this method.</param>
    /// <param name="parameter">An optional parameter for the converter. Not used in this method.</param>
    /// <param name="culture">The culture information to be used in the conversion. Not used in this method.</param>
    /// <return>Returns true if the input value is a non-null, non-empty string; otherwise, false.</return>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    /// Converts a boolean value back to a string. This method is not implemented and will throw a NotImplementedException if called.
    /// <param name="value">The input value to be converted back, expected to be a boolean.</param>
    /// <param name="targetType">The target type for the conversion. Not used in this method.</param>
    /// <param name="parameter">An optional parameter for the converter. Not used in this method.</param>
    /// <param name="culture">The culture information to be used in the conversion. Not used in this method.</param>
    /// <return>This method always throws a NotImplementedException.</return>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}