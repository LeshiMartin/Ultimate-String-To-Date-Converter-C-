using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace ultimate_string_to_date_converter
{
#nullable enable
    public static class StringToDateConverter
    {
        static StringToDateConverter()
        {
            DateFormats = GetDateFormatConfig();
        }
        private static IDictionary<string, string> DateFormats { get; set; }

        public static void InsertFormats(params string[] formats)
        {
            foreach (var format in formats)
            {
                if (!DateFormats.ContainsKey(format))
                    DateFormats.Add(format, format);
            }
        }
        public static DateTime ParseToDate(string dateString, IFormatProvider? provider = null, DateTimeStyles? dateStyle = null)
        {
            provider = provider ?? CultureInfo.InvariantCulture;
            dateStyle = dateStyle ?? DateTimeStyles.None;
            
            
            foreach (var kvp in DateFormats)
            {
                if (DateTime.TryParseExact(dateString, kvp.Value, provider, dateStyle.Value, out DateTime date))
                    return date;
            }
            throw new ArgumentException("The provided string was not found in any of the formats");
        }
        public static bool TryParseDate(string dateString, out DateTime returnDate, IFormatProvider? provider = null, DateTimeStyles? dateStyle = null)
        {
            if (dateString.Contains("tt"))
            {
                dateString = dateString.Replace("tt", "");
                dateString = dateString.Trim();
            }
            provider = provider ?? CultureInfo.InvariantCulture;
            dateStyle = dateStyle ?? DateTimeStyles.None;
            bool converted = false;
            returnDate = DateTime.UtcNow;
            foreach (var kvp in DateFormats)
            {
                if (DateTime.TryParseExact(dateString, kvp.Value, provider, dateStyle.Value, out DateTime date))
                {
                    converted = true;
                    returnDate = date;
                    break;
                }
            }

            return converted;
        }
        private static IDictionary<string, string> GetDateFormatConfig()
        {
            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("date-formats.json");
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            var js = reader.ReadToEndAsync().Result;
            if (js is null)
                return new Dictionary<string, string>();
            var formats = JsonConvert.DeserializeObject<Dictionary<string, string>>(js);
            return formats;
        }
    }
}
