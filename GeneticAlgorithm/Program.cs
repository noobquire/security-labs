using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Algorithm = GeneticSharp.Domain.GeneticAlgorithm;
using System;
using SecurityLabs;
using CipherUtils;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticAlgorithm
{
    internal class Program
    {
        enum SubstitutionType
        {
            MonoSubstitution,
            PolySubstitution
        }

        static void Main()
        {
            var text = Utils.GetString("text to decipher");

            var subType = (SubstitutionType)Utils.GetInt("substitution type (0 = mono, 1 = poly)", 1, 0);

            switch (subType)
            {
                case SubstitutionType.MonoSubstitution:
                    MonoSubstitution(text);
                    return;
                case SubstitutionType.PolySubstitution:
                    PolySubstitution(text);
                    return;
            };
        }

        private static void PolySubstitution(string text)
        {
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

            Console.WriteLine("Columns: ");
            var keys = new List<string>();
            // apply GA to each column to try get best substitution key
            foreach (var column in columns)
            {
                Console.WriteLine(column);
                var key = DecipherMonoSubstitution(column);
                keys.Add(key);
            }
            Console.WriteLine();

            (char plain, char cipher)[][] knownLetters =
            {
               new[] { ('C', 'U'), ('T', 'Z'), ('I', 'B'), ('B', 'E'), ('Y', 'X'), ('S', 'M'), ('R', 'L'), ('D', 'I'), ('L', 'O'), ('O', 'K'), ('N', 'F'), ('P', 'N'), ('E', 'J'), ('A', 'T'), ('H', 'P'), ('K', 'A'), ('W', 'Q'), ('Q', 'D'), ('G', 'S'), ('U', 'R'), ('M', 'H'), ('F', 'Y'), ('J', 'V') },
               new[] { ('A', 'Y'), ('C', 'J'), ('T', 'K'), ('I', 'D'), ('S', 'O'), ('H', 'I'), ('E', 'E'), ('O', 'M'), ('N', 'T'), ('F', 'U'), ('D', 'B'), ('B', 'Q'), ('U', 'Z'), ('R', 'H'), ('G', 'N'), ('M', 'X'), ('L', 'L'), ('Y', 'V'), ('M', 'P'), ('W', 'A'), ('Z', 'C'), ('K', 'W'), ('P', 'R') },
               new[] { ('P', 'X'), ('L', 'T'), ('A', 'O'), ('T', 'R'), ('L', 'T'), ('E', 'A'), ('N', 'U'), ('S', 'Y'), ('U', 'G'), ('B', 'Q'), ('O', 'I'), ('I', 'H'), ('D', 'P'), ('H', 'Z'), ('K', 'L'), ('Y', 'M'), ('R', 'D'), ('W', 'W'), ('G', 'K'), ('X', 'B'), ('F', 'S'), ('C', 'F'), ('M', 'J') },
               new[] { ('I', 'C'), ('L', 'H'), ('P', 'E'), ('L', 'H'), ('A', 'L'), ('H', 'K'), ('G', 'P'), ('U', 'X'), ('O', 'U'), ('E', 'N'), ('N', 'A'), ('C', 'G'), ('S', 'Q'), ('R', 'S'), ('B', 'J'), ('T', 'Y'), ('W', 'Z'), ('X', 'V'), ('N', 'T'), ('Y', 'M'), ('J', 'B'), ('M', 'D'), ('D', 'O'), ('V', 'W'), ('F', 'R'), ('Z', 'I') }
            };


            for (int i = 0; i < keys.Count(); i++)
            {
                string key = keys[i];
                string newKey = key;
                foreach (var knownLetter in knownLetters[i])
                {
                    newKey = newKey.ChangeKeyChar(knownLetter.plain, knownLetter.cipher);
                }
                keys[i] = newKey;
            }

            Console.WriteLine("\nKeys: ");
            foreach (var key in keys)
            {
                Console.WriteLine(key);
            }
            Console.WriteLine();

            var decipheredText = PolySubstitutionCipher.Decrypt(keys.ToArray(), text);

            for (int i = 1; i <= decipheredText.Length; i++)
            {
                if ((i - 1) % 100 == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine(text.Substring(i - 1, text.Substring(i - 1).Length >= 100 ? 100 : text.Substring(i - 1).Length % 100));
                    Console.WriteLine(string.Concat(Enumerable.Repeat("1234", 25)));
                }

                char ch = text[i - 1];

                bool firstAlphabet = i % 4 == 1 && knownLetters[0].Any(kl => kl.cipher == ch);
                bool secondAlphabet = i % 4 == 2 && knownLetters[1].Any(kl => kl.cipher == ch);
                bool thirdAlphabet = i % 4 == 3 && knownLetters[2].Any(kl => kl.cipher == ch);
                bool fourthAlphabet = i % 4 == 0 && knownLetters[3].Any(kl => kl.cipher == ch);

                if (firstAlphabet || secondAlphabet || thirdAlphabet || fourthAlphabet)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(decipheredText[i - 1]);
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(decipheredText[i - 1]);
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine + decipheredText);
            Console.WriteLine($"Text fitness: {FrequencyAnalysis.GetQuadgramScore(decipheredText)}");
        }

        private static void MonoSubstitution(string text)
        {
            var sw = new Stopwatch();
            Console.WriteLine("GA running...");
            sw.Start();
            var key = DecipherMonoSubstitution(text);
            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");
            var decipheredText = MonoSubstitutionCipher.Decrypt(key, text);
            Console.WriteLine(decipheredText);
        }

        /// <summary>
        /// Get monoalphabetic substitution key with genetic algorithm.
        /// </summary>
        /// <param name="text">Ciphertext</param>
        /// <returns>Key alphabet</returns>
        private static string DecipherMonoSubstitution(string text)
        {
            var selection = new EliteSelection();
            var crossover = new PositionBasedCrossover();
            var mutation = new TworsMutation();
            var fitness = new SubstitutionFitness(text);
            var chromosome = new SubstitutionChromosome();
            var population = new Population(200, 250, chromosome);

            var ga = new Algorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(500),
                CrossoverProbability = 0.45f,
                MutationProbability = 0.8f
            };

            ga.Start();
            Console.WriteLine($"Best solution: {ga.BestChromosome} with {ga.BestChromosome.Fitness} fitness");
            return ((SubstitutionChromosome)ga.BestChromosome).ToString();
        }
    }
}
