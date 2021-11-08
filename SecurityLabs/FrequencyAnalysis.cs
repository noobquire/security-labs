using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecurityLabs
{
    public static class FrequencyAnalysis
    {
        public static readonly Dictionary<string, double> QuadgramOccurences;

        /// <summary>
        /// Relative frequency of each character appearing in English language.
        /// </summary>
        public static readonly Dictionary<char, decimal> EnglishLetterFrequencies = new Dictionary<char, decimal>()
        {
            ['a'] = 0.082m,
            ['b'] = 0.015m,
            ['c'] = 0.028m,
            ['d'] = 0.043m,
            ['e'] = 0.14m,
            ['f'] = 0.022m,
            ['g'] = 0.02m,
            ['h'] = 0.061m,
            ['i'] = 0.07m,
            ['j'] = 0.0015m,
            ['k'] = 0.0077m,
            ['l'] = 0.04m,
            ['m'] = 0.024m,
            ['n'] = 0.067m,
            ['o'] = 0.075m,
            ['p'] = 0.019m,
            ['q'] = 0.00095m,
            ['r'] = 0.06m,
            ['s'] = 0.063m,
            ['t'] = 0.091m,
            ['u'] = 0.028m,
            ['v'] = 0.0098m,
            ['w'] = 0.024m,
            ['x'] = 0.0015m,
            ['y'] = 0.02m,
            ['z'] = 0.00074m
        };

        static FrequencyAnalysis()
        {
            QuadgramOccurences = new Dictionary<string, double>();
            var lines = File.ReadAllLines("english_quadgrams.txt");
            foreach (var line in lines)
            {
                var quadgram = line.Split(' ')[0];
                double occurences = int.Parse(line.Split(' ')[1]);
                QuadgramOccurences[quadgram] = occurences;
            }
        }

        /// <summary>
        /// Get corelation between letter frequency in given text and average English text frequency.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <returns>Relative corelation.</returns>
        public static decimal GetRelativeFrequencyCorelation(string text)
        {
            decimal relativeFrequencyCorelation = 0;
            foreach (var charFrequency in GetCharFrequencies(text).OrderBy(c => c.Value))
            {
                var letterOccurs = EnglishLetterFrequencies.TryGetValue(charFrequency.Key, out decimal letterFrequency);
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
        public static Dictionary<char, decimal> GetCharFrequencies(string text)
        {
            return text
                .ToLower()
                .Distinct()
                .ToDictionary(c => c,
                    c => 1000 * text
                         .Count(letter => letter == c)
                         / (decimal)text.Length);
        }

        public static Dictionary<string, int[]> GetNGramDistances(int n, string text)
        {
            var ngrams = Enumerable
                .Range(0, text.Length - n - 1)
                .Select(i => text.Substring(i, n));
            var ngramDistances = ngrams
                .Distinct()
                .Where(t => ngrams.Count(tr => tr == t) > 1)
                .OrderByDescending(t => ngrams.Count(tr => tr == t))
                .ToDictionary(t => t, // repeating trigram
                t =>
                {
                    var ngramEntriesIndexes = text.AllIndexesOf(t).ToArray();
                    var distances = new int[ngramEntriesIndexes.Length - 1];
                    for (int i = 1; i < ngramEntriesIndexes.Count(); i++)
                    {
                        distances[i - 1] = ngramEntriesIndexes[i] - ngramEntriesIndexes[i - 1];
                    }
                    return distances.OrderBy(d => d).ToArray();
                });
            return ngramDistances;
        }

        public static double QuadgramScore(string text)
        {
            var quadgrams = Enumerable
                .Range(0, text.Length - 3)
                .Select(i => text.Substring(i, 4))
                .ToArray();
            var quadgramOccurences = new Dictionary<string, double>();

            foreach (var quadgram in quadgrams)
            {
                if (QuadgramOccurences.TryGetValue(quadgram, out double occurences))
                {
                    quadgramOccurences[quadgram] = occurences;
                }
            }

            var averageOccurences = quadgramOccurences.Average(quadgram => quadgram.Value);

            return averageOccurences;
        }
    }
}
