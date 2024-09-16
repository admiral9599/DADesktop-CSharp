using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DriveAdviser.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");

        public static Version ToVersion(this string txtVersion)
        {
            Version version = null;
            Version.TryParse(txtVersion, out version)
                ;

            return version;
        }

        public static bool IsValidEmail(this string emailAddress)
        {
            try
            {
                var regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
                var matches = Regex.Match(emailAddress, regexPattern);
                return matches.Success;
            }
            catch (Exception)
            {

                return false;
            }
          
        }
        public static bool IsChinese(this char c)
        {
            return cjkCharRegex.IsMatch(c.ToString());
        }

        public static string ToFormattedJson(this string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }

    
}