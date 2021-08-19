using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrokeForEgypt.Common
{
    public static class RandomGenerator
    {
        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size    
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }

            return builder.ToString();
        }

        // Generate a random key    
        public static string RandomKey()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, true));
            return builder.ToString();
        }

        public static List<T> Randomize<T>(this IEnumerable<T> source)
        {
            List<T> array = source.OrderBy(a => Guid.NewGuid()).ToList();
            // randomize indexes (several approaches are possible)
            return array;
        }
    }
}
