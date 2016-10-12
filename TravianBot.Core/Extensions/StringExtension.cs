using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core.Extensions
{
    public static class StringExtension
    {
        public static string Substring(this string str, string left, string right)
        {
            int startIndex = str.IndexOf(left) + left.Length;
            int endIndex = str.IndexOf(right);
            return str.Substring(startIndex, endIndex - startIndex);
        }

        public static string Substring(this string str, string left)
        {
            int startIndex = str.IndexOf(left) + left.Length;
            return str.Substring(startIndex, str.Length - startIndex);
        }

        public static int GetNumber(this string str)
        {
            return int.Parse(string.Concat(str.Where(c => c >= 48 && c <= 57)));
        }

        public static string Combine(this string url, params string[] subUrl)
        {
            var result = new Uri(url);
            for (int i = 0; i < subUrl.Length; i++)
            {
                result = new Uri(result, subUrl[i]);
            }
            return result.AbsoluteUri;
        }

        public static string Combine(this string url, params KeyValuePair<string, string>[] keyValues)
        {
            var uri = new Uri(url);
            var result = string.IsNullOrEmpty(uri.Query) ? uri.AbsoluteUri + "?" : uri.AbsoluteUri;
            foreach (var keyValue in keyValues)
            {
                result += string.Format($"{keyValue.Key}={keyValue.Value}&");
            }
            return result;
        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }

        public static Uri ToUri(this string s)
        {
            if (s.IsNullOrEmptyOrWhiteSpace())
                return null;
            
            var url = s.StartsWith("http://") ? s : string.Format($"http://{s}");
            return new Uri(url);
        }
    }
}
