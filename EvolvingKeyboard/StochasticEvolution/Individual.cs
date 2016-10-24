using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvingKeyboard.StochasticEvolution
{
    public class Individual<GENE_TYPE> where GENE_TYPE : IGene
    {
        public List<GENE_TYPE> DNA
        {
            get;
            private set;
        }

        public Individual(List<GENE_TYPE> dna)
        {
            DNA = dna;
        }
        Individual(int dnaSize)
        {
            DNA = new List<GENE_TYPE>(dnaSize);
        }

        public static List<GENE_TYPE> GenerateDNAWithoutDoublons(int size, Func<object[], GENE_TYPE> generateGeneRoutine, params object[] routineParams)
        {
            var newDNA = new List<GENE_TYPE>(size);
            for (int i = 0; i < newDNA.Count; ++i)
            {
                var newGene = (GENE_TYPE)generateGeneRoutine(routineParams);
                if (newDNA.Find((gene) => { return gene.Equals(newGene); }) != null)
                    i--;
                else
                    newDNA[i] = newGene;
            }
            return newDNA;
        }

        public void ApplyMutation(float geneMutationCoef)
        {
            ApplyMutation(-1, -1, geneMutationCoef);
        }
        public void ApplyMutation(int minGeneChangeCount, int maxGeneChangeCount, float geneMutationCoef)
        {
            System.Diagnostics.Debug.Assert(DNA != null);
            System.Diagnostics.Debug.Assert(maxGeneChangeCount >= DNA.Count);
            HashSet<int> mutatedGenesIndexes = new HashSet<int>(); // To avoid mutation of the same genes

            var randomGenerator = new Random();
            int finalGeneChange = 0;
            if (minGeneChangeCount == -1 || maxGeneChangeCount == -1)
                finalGeneChange = DNA.Count;
            else
             finalGeneChange = randomGenerator.Next(minGeneChangeCount, maxGeneChangeCount);
            for (int geneChangeCount = 0; geneChangeCount < finalGeneChange; ++geneChangeCount)
            {
                int changeIndex = 0;
                do
                {
                    changeIndex = randomGenerator.Next(0, DNA.Count);
                } while (mutatedGenesIndexes.Contains(changeIndex));
                mutatedGenesIndexes.Add(changeIndex);
                DNA[changeIndex].Mutate(geneMutationCoef);
            }
        }

        //public 
    }
}
