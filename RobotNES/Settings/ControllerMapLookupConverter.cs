using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NESCore;

namespace RobotNES.Settings;

/// <summary>
/// Reverse-looks-up ControllerMappings.ControllerMap (keyed by GamepadButtons, valued by
/// IController.Keys) to find the controller button currently bound to a given NES button.
/// The bound value is the dictionary, the ConverterParameter is the NES button name
/// (e.g. "UP"). Returns the controller button name, or "Unbound" when nothing maps to it,
/// and <see cref="DependencyProperty.UnsetValue"/> on malformed input.
/// </summary>
public class ControllerMapLookupConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IDictionary map || parameter is not string nesButtonName)
            return DependencyProperty.UnsetValue;

        if (!Enum.TryParse<IController.Keys>(nesButtonName, out var nesKey))
            return DependencyProperty.UnsetValue;

        foreach (DictionaryEntry entry in map)
        {
            if (Equals(entry.Value, nesKey))
                return entry.Key;
        }

        return "Unbound";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
