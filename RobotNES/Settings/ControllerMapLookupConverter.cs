using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RobotNES.Settings;

public class ControllerMapLookupConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IDictionary map || parameter is not string keyName)
            return DependencyProperty.UnsetValue;

        var keyType = KeyTypeOf(map);
        if (keyType is not { IsEnum: true })
            return DependencyProperty.UnsetValue;

        object key;
        try
        {
            key = Enum.Parse(keyType, keyName);
        }
        catch (ArgumentException)
        {
            return DependencyProperty.UnsetValue;
        }

        return map.Contains(key) ? map[key] : DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static Type? KeyTypeOf(IDictionary map)
    {
        foreach (var declaredInterface in map.GetType().GetInterfaces())
        {
            if (!declaredInterface.IsGenericType)
                continue;

            if (declaredInterface.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
                return declaredInterface.GetGenericArguments()[0];
        }

        return null;
    }
}
