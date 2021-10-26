using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class AscendingSelection : SelectionBase
    {
        public AscendingSelection() : base(2)
        {
        }

        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            return generation.Chromosomes.OrderBy((IChromosome c) => c.Fitness).Take(number).ToList();
        }
    }
}
