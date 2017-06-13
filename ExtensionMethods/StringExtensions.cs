using System;
using System.Text.RegularExpressions;
using Chronic;

namespace WeatherBot.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool IsValidCity(this String str)
        {
            Regex regex = new Regex("^[a-zA-Z]+,[A-Z][A-Z]$");
            return regex.IsMatch(str.ReplaceAll(" ", ""));
        }

        public static string GetHTTPEncoded(this string str)
        {
            return str.ReplaceAll(" ", "%20");
        }
    }
}