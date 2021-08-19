using System;
using System.Text;
using System.Text.RegularExpressions;

namespace StrokeForEgypt.Common
{
    public static class EncodeManager
    {
        public static string Base64Encode(string plainText)
        {
            if (!IsBase64String(plainText))
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            return plainText;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (IsBase64String(base64EncodedData))
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            return base64EncodedData;
        }

        public static bool IsBase64String(string base64)
        {
            base64 = base64.Trim();
            return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
