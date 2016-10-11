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

        public static List<GENE_TYPE> GenerateDNAWithoutDoublons(int size)
        {
            var newDNA = new List<GENE_TYPE>(size);
            for (int i = 0; i < newDNA.Count; ++i)
            {
                var newGene = (GENE_TYPE)IGene.GenerateRandom();
                if (newDNA.Find((gene) => { return gene.Equals(newGene); }) != null)
                    i--;
                else
                    newDNA[i] = newGene;
            }
            return newDNA;
        }

        public void ApplyMutation(int minGeneChangeCount, int maxGeneChangeCount, float geneMutationCoef)
        {
            var randomGenerator = new Random();
            System.Diagnostics.Debug.Assert(DNA != null);
            int finalGeneChange = randomGenerator.Next(minGeneChangeCount, maxGeneChangeCount);
            for (int geneChangeCount = 0; geneChangeCount < finalGeneChange; ++geneChangeCount)
            {
                int changeIndex = randomGenerator.Next(0, DNA.Count);
                DNA[changeIndex].Mutate(geneMutationCoef);
            }
        }

        //public 
    }
}
