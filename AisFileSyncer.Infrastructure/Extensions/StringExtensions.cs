using System;
using System.Text;

namespace AisFileSyncer.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string s)
        {
            return s == null ? true : string.IsNullOrEmpty(s.Trim());
        }

        public static string ThrowIfNullOrEmpty(this string s, string message = "")
        {
            return s.IsEmpty() ? throw new ArgumentNullException(message) : s;
        }

        public static bool IsNotEmpty(this string s)
        {
            return !IsEmpty(s);
        }

        public static string EmptyIfNull(this string s)
        {
            return s == null ? string.Empty : s;
        }

        public static string[] SplitByString(this string s, string delimiter)
        {
            return s.Split(new string[] { delimiter }, StringSplitOptions.None);
        }

        public static string ToBase64(this Encoding encoding, string text)
        {
            if (text == null)
            {
                return null;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        public static string FromBase64(this Encoding encoding, string encodedText)
        {
            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = Convert.FromBase64String(encodedText);
            return encoding.GetString(textAsBytes);
        }

        public static byte[] ToBytes(this string text)
        {
            return text.IsEmpty()
                ? null
                : Convert.FromBase64String(text);
        }
    }

}
