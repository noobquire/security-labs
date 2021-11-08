using CipherUtils;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using SecurityLabs;

namespace GeneticAlgorithm
{
    internal class SubstitutionFitness : IFitness
    {
        private readonly string cipherText;

        public SubstitutionFitness(string cipherText)
        {
            this.cipherText = cipherText;
        }

        public double Evaluate(IChromosome chromosome)
        {
            var abcChromosome = chromosome as SubstitutionChromosome;
            var decryptedText = MonoSubstitutionCipher.Decrypt(abcChromosome.ToString(), cipherText);
            return FrequencyAnalysis.GetLetterOverallFrequency(decryptedText.ToLower());
        }
    }
}
