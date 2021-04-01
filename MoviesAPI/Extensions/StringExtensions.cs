using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI
{
    public static class StringExtensions
    {
        public static bool IsLessThan(this string self, int minLenght)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return true;
            }
            return self.Trim().Length < minLenght;

        }

        public static bool IsLenght(this string self, int lenght)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return false;
            }
            return self.Trim().Length == lenght;

        }

        public static bool IsMoreThan(this string self, int minLenght)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return false;
            }
            return self.Trim().Length > minLenght;

        }

        public static string RemoveAccents(this string self)
        {
            return Encoding.ASCII.GetString(Encoding.GetEncoding(1251).GetBytes(self)).ToUpper();
        }

        public static string ToTitleCase(this string title)
        {
            if (title == null) return "";
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
