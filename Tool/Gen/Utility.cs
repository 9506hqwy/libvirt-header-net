namespace Gen;

using System.Globalization;
using System.Text.RegularExpressions;

internal static class Utility
{
    internal static string ToClassName(string value)
    {
        return Utility.ToUpperCamelCase(value);
    }

    internal static string ToFieldName(string value)
    {
        return Utility.ToLowerCamelCase(value);
    }

    internal static string ToPropertyName(string value)
    {
        return Utility.ToUpperCamelCase(value);
    }

    private static string ToLowerCamelCase(string value)
    {
        var terms = Regex.Replace(value, @"([A-Z]+)", "_$1")
            .Trim('_')
            .Split('_')
            .Select(t => t.ToLowerInvariant())
            .Select((t, i) => i == 0 ? t : Utility.ToCaption(t));
        return string.Join(string.Empty, terms);
    }

    private static string ToUpperCamelCase(string value)
    {
        var terms = Regex.Replace(value, @"([A-Z]+)", "_$1")
            .Trim('_')
            .Split('_')
            .Select(Utility.ToCaption);
        return string.Join(string.Empty, terms);
    }

    private static string ToCaption(string value)
    {
        var caption = value
            .ToCharArray()
            .Select((ch, i) => i == 0 ? char.ToUpper(ch, CultureInfo.CurrentCulture) : char.ToLower(ch, CultureInfo.CurrentCulture))
            .ToArray();
        return new string(caption);
    }
}
