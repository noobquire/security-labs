using System.Collections.Generic;
using System.Linq;

namespace SecurityLabs
{
    public static class FrequencyAnalysis
    {
        /// <summary>
        /// Relative frequency of each character appearing in English language.
        /// </summary>
        public static Dictionary<char, double> EnglishLetterFrequencies = new Dictionary<char, double>()
        {
            ['a'] = 0.082,
            ['b'] = 0.015,
            ['c'] = 0.028,
            ['d'] = 0.043,
            ['e'] = 0.14,
            ['f'] = 0.022,
            ['g'] = 0.02,
            ['h'] = 0.061,
            ['i'] = 0.07,
            ['j'] = 0.0015,
            ['k'] = 0.0077,
            ['l'] = 0.04,
            ['m'] = 0.024,
            ['n'] = 0.067,
            ['o'] = 0.075,
            ['p'] = 0.019,
            ['q'] = 0.00095,
            ['r'] = 0.06,
            ['s'] = 0.063,
            ['t'] = 0.091,
            ['u'] = 0.028,
            ['v'] = 0.0098,
            ['w'] = 0.024,
            ['x'] = 0.0015,
            ['y'] = 0.02,
            ['z'] = 0.00074
        };

        /// <summary>
        /// Get corelation between letter frequency in given text and average English text frequency.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <returns>Relative corelation.</returns>
        public static double GetRelativeFrequencyCorelation(string text)
        {
            double relativeFrequencyCorelation = 0;
            foreach (var charFrequency in GetCharFrequencies(text).OrderBy(c => c.Value))
            {
                var letterOccurs = EnglishLetterFrequencies.TryGetValue(charFrequency.Key, out double letterFrequency);
                if (letterOccurs)
                {
                    relativeFrequencyCorelation += letterFrequency * charFrequency.Value;
                }
            }
            return relativeFrequencyCorelation;
        }

        /// <summary>
        /// Get relative frequency of each character appearing in text.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <returns>Dictionary with relative frequency for each character.</returns>
        public static Dictionary<char, double> GetCharFrequencies(string text)
        {
            return text
                .ToLower()
                .Distinct()
                .ToDictionary(c => c,
                    c => 1000 * text
                         .Count(letter => letter == c)
                         / (double)text.Length);
        }

        public static Dictionary<string, int[]> GetTrigramDistances(string text)
        {
            var trigrams = Enumerable.Range(0, text.Length - 2).Select(i => text.Skip(i).Take(3));
        }
    }
}
