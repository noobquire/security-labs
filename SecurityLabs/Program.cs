using System;
using System.Linq;

namespace SecurityLabs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = Utils.GetString("text");
            for (int i = 2; i < 6; i++)
            {
                Console.WriteLine($"NGRAM DISTANCE ANALYSIS, N = {i}");
                var trigramDistances = FrequencyAnalysis.GetNGramDistances(i, text);
                foreach (var distance in trigramDistances.Take(5))
                {
                    Console.WriteLine($"{distance.Key}: {string.Join(", ", distance.Value)}");
                }
                Console.WriteLine();
            }
            var keyLength = Utils.GetInt("key length", text.Length, 1);

            var rows = text.Chunk(keyLength).ToArray();
            var columns = Enumerable
                .Range(0, keyLength)
                .Select(i => rows.Select(row => row
                        .ToCharArray()
                        .ElementAt(i))
                    .ToArray())
                .Select(columnLetters => new string(columnLetters));

            // apply GA to each column to get best substitution

            // combine columns and get text
        }
    }
}
