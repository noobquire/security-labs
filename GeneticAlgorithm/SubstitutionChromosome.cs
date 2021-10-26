using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    internal class SubstitutionChromosome : ChromosomeBase
    {
        private const int MIN_GENE_VALUE = 'A';
        private const int MAX_GENE_VALUE = 'Z';
        private const int ALPHABET_LENGTH = 26;
        private readonly string originalValue;

        public SubstitutionChromosome() : base(ALPHABET_LENGTH)
        {

            var chars = RandomizationProvider
                        .Current
                        .GetUniqueInts(ALPHABET_LENGTH, MIN_GENE_VALUE, MAX_GENE_VALUE + 1)
                        .Select(c => (char)c)
                        .ToArray();
            originalValue = new string(chars);
            for (int i = 0; i < ALPHABET_LENGTH; i++)
            {
                this.ReplaceGene(i, new Gene(originalValue[i]));
            }
        }

        public override IChromosome CreateNew()
        {
            return new SubstitutionChromosome();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene((char)RandomizationProvider.Current.GetInt(MIN_GENE_VALUE, MAX_GENE_VALUE));
        }

        public int LetterIndex(char letter)
        {
            return this.GetGenes()
                .ToList()
                .FindIndex(0, gene => (char)gene.Value == letter);
        }
    }
}
