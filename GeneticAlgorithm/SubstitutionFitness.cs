using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using SecurityLabs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XorDecipher;

namespace GeneticAlgorithm
{
    internal class SubstitutionFitness : IFitness
    {
        private readonly string cipherText;
        public static readonly Dictionary<string, double> QuadgramProbabilities;
        public static readonly double NormalFitness;

        static SubstitutionFitness()
        {
            QuadgramProbabilities = new Dictionary<string, double>();
            var lines = File.ReadAllLines("english_quadgrams.txt");
            long totalOccurences = lines.Sum(l => long.Parse(l.Split(' ')[1]));
            foreach (var line in lines)
            {
                var quadgram = line.Split(' ')[0];
                double occurences = int.Parse(line.Split(' ')[1]);
                QuadgramProbabilities[quadgram] = occurences / totalOccurences;
            }
            var normalFitness = QuadgramProbabilities.Average(quadgram => Math.Log10(quadgram.Value));
            NormalFitness = normalFitness;
        }

        public SubstitutionFitness(string cipherText)
        {
            this.cipherText = cipherText;
        }
        public double Evaluate(IChromosome chromosome)
        {
            var abcChromosome = chromosome as SubstitutionChromosome;
            var decryptedText = MonoSubstitutionCipher.Decrypt(abcChromosome.ToString(), cipherText);
            return QuadgramScore(decryptedText);
        }

        private double QuadgramScore(string text)
        {
            var quadgrams = Enumerable
                .Range(0, text.Length - 3)
                .Select(i => new string(text
                    .Skip(i)
                    .Take(4)
                    .ToArray()))
                .ToArray();
            var quadgramProbabilities = new Dictionary<string, double>();

            foreach (var quadgram in quadgrams)
            {
                if(QuadgramProbabilities.TryGetValue(quadgram, out double probability))
                {
                    quadgramProbabilities[quadgram] = probability;
                }
            }

            var textFitness = quadgramProbabilities.Average(quadgram => Math.Log10(quadgram.Value));

             var score = Math.Abs(textFitness - NormalFitness) / NormalFitness;
             return Math.Abs(score);
        }
    }
}
