using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecurityLabs
{
    public static class FrequencyAnalysis
    {
        public static readonly Dictionary<string, double> QuadgramOccurences;
        public static readonly Dictionary<char, int> LetterOccurencesInLab;
        /// <summary>
        /// Relative frequency of each character appearing in English language.
        /// </summary>
        /*public static readonly Dictionary<char, int> EnglishLetterOccurences = new Dictionary<char, int>()
        {
            ['e'] = 390395169,
            ['t'] = 282039486,
            ['a'] = 248362256,
            ['o'] = 235661502,
            ['i'] = 214822972,
            ['n'] = 214319386,
            ['s'] = 196844692,
            ['h'] = 193607737,
            ['r'] = 184990759,
            ['d'] = 134044565,
            ['l'] = 125951672,
            ['u'] = 88219598,
            ['c'] = 79962026,
            ['m'] = 79502870,
            ['f'] = 72967175,
            ['w'] = 69069021,
            ['g'] = 61549736,
            ['y'] = 59010696,
            ['p'] = 55746578,
            ['b'] = 47673928,
            ['v'] = 30476191,
            ['k'] = 22969448,
            ['x'] = 5574077,
            ['j'] = 4507165,
            ['q'] = 3649838,
            ['z'] = 2456495
        };
        */

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

            var text = File.ReadAllText("lab1_plaintext.txt");
            var letters = text
                .ToLower()
                .ToCharArray()
                .Where(c => char.IsLetter(c));
            LetterOccurencesInLab = letters
                 .Distinct()
                 .ToDictionary(l => l,
                 l => letters.Count(letter => letter == l));
        }

        /// <summary>
        /// Get corelation between letter frequency in given text and average English text frequency.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <returns>Relative corelation.</returns>
        public static double GetLetterOverallFrequency(string text)
        {
            double relativeFrequencyCorelation = 0;
            foreach (var charFrequency in GetCharFrequencies(text).OrderBy(c => c.Value))
            {
                var letterOccurs = LetterOccurencesInLab.TryGetValue(charFrequency.Key, out int letterFrequency);
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

        public static double GetQuadgramScore(string text)
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
