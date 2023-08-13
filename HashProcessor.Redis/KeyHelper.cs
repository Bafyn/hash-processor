using System.Globalization;

namespace HashProcessor.Redis;

public static class KeyHelper
{
    public static DateTime ParseDateKey(string dateString)
    {
        return DateTime.ParseExact(dateString, Common.Constants.DateFormat, CultureInfo.InvariantCulture);
    }

    public static string GetDateKey(DateTime date)
    {
        return date.ToString(Common.Constants.DateFormat, CultureInfo.InvariantCulture);
    }
}
