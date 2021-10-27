using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Algorithm = GeneticSharp.Domain.GeneticAlgorithm;
using System;
using SecurityLabs;
using System.Timers;
using System.Diagnostics;
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
            var mutation = new DisplacementMutation();
            var fitness = new SubstitutionFitness(text);
            var chromosome = new SubstitutionChromosome();
            var population = new Population(20, 200, chromosome);

            var ga = new Algorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(200),
                MutationProbability = 0.45f
            };

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
            Console.WriteLine(MonoSubstitutionCipher.Decrypt(((SubstitutionChromosome)ga.BestChromosome).ToString(), text));
        }
    }
}
