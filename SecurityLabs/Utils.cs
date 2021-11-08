using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityLabs
{
    public static class Utils
    {
        public static string GetString(string name, bool allowEmpty = false)
        {
            while (true)
            {
                Console.WriteLine($"Please input {name}: ");
                string input = Console.ReadLine();
                if (allowEmpty || string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input must not be empty!");
                    continue;
                }
                return input;
            }
        }

        public static int GetInt(string name, int max = int.MaxValue, int min = 0)
        {
            while (true)
            {
                Console.WriteLine($"Please input {name} between {min} and {max}: ");
                var result = int.TryParse(Console.ReadLine(), out int value);
                if (!result)
                {
                    Console.WriteLine("Invalid input, try again");
                    continue;
                }
                if (min > value || max < value)
                {
                    Console.WriteLine($"Provided value is not between {min} and {max}, try again");
                }
                return value;
            }
        }

        public static IEnumerable<string> Chunk(this string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static string Shift(this string s, int count)
        {
            return s.Remove(0, count) + s.Substring(0, count);
        }

        public static IEnumerable<IEnumerable<string>> TransponateStringMatrix(IEnumerable<IEnumerable<string>> matrix)
        {
            var arrayMatrix = matrix.Select(col => col.ToArray()).ToArray();
            var newMatrix = new string[arrayMatrix[0].Length][];
            for (int i = 0; i < arrayMatrix[0].Length; i++)
            {
                for (int j = 0; j < arrayMatrix.Length; j++)
                {
                    newMatrix[i][j] = arrayMatrix[j][i];
                }
            }
            return newMatrix;
        }

        public static string ToNormalizedPlaintext(this string text)
        {
            char minValue = 'A';
            char maxValue = 'Z';

            bool isEnglishLetter(char c) => c >= minValue && c <= maxValue;
            return new string(text
                .ToUpperInvariant()
                .ToCharArray()
                .Where(c => isEnglishLetter(c))
                .ToArray());
        }

        public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }
    }
}
