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

        static void Main(string[] args)
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

            // apply GA to each column to get best substitution key
            foreach (var column in columns)
            {
                Console.WriteLine(column);
                var key = DecipherMonoSubstitution(column);
                keys.Add(key);
            }
            Console.WriteLine();

            var decipheredText = PolySubstitutionCipher.Decrypt(keys.ToArray(), text);

            Console.WriteLine(decipheredText);
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
