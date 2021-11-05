using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Algorithm = GeneticSharp.Domain.GeneticAlgorithm;
using System;
using SecurityLabs;
using XorDecipher;

namespace GeneticAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = Utils.GetString("text to decipher");

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

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine($"Best solution: {ga.BestChromosome} with {ga.BestChromosome.Fitness} fitness");
            Console.WriteLine(MonoSubstitutionCipher.Decrypt(((SubstitutionChromosome)ga.BestChromosome).ToString(), text));
        }
    }
}
