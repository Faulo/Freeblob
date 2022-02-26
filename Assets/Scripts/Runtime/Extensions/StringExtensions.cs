using System.Collections.Generic;

namespace FreeBlob.Extensions {
    public static class StringExtensions {
        public static string Format(this string formatString, IDictionary<string, string> dictionary) {
            foreach (var element in dictionary) {
                formatString = formatString.Replace("{" + element.Key + "}", element.Value);
            }
            return formatString;
        }
    }
}