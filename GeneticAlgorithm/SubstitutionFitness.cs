using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XorDecipher;

namespace GeneticAlgorithm
{
    internal class SubstitutionFitness : IFitness
    {
        private readonly string cipherText;
        public static readonly Dictionary<string, double> QuadgramOccurences;
        public static readonly double AverageOccurence;

        static SubstitutionFitness()
        {
            QuadgramOccurences = new Dictionary<string, double>();
            var lines = File.ReadAllLines("english_quadgrams.txt");
            foreach (var line in lines)
            {
                var quadgram = line.Split(' ')[0];
                double occurences = int.Parse(line.Split(' ')[1]);
                QuadgramOccurences[quadgram] = occurences;
            }
            AverageOccurence = QuadgramOccurences.Average(quadgram => quadgram.Value);
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
