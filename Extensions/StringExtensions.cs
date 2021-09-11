using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CovertWorker.Extensions
{
    public static class StringExtensions
    {
        static Regex pattern = new Regex(@"[{](\w+?)([:].+?)?[}]");

        public static string Format(
            this string format,
            IDictionary<string, object> values)
        {
            foreach (Match match in pattern.Matches(format))
            {
                var name = match.Groups[1].Value;
                var value = values[name];

                var str = match.Value.Replace(name, "0");
                str = string.Format(str, value);

                format = format.Replace(match.Value, str);
            }

            return format;
        }
    }
}
