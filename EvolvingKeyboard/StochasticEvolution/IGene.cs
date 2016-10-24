using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvingKeyboard.StochasticEvolution
{
    public interface IGene
    {
        bool Equals(IGene gene);
        IGene GenerateRandom();
        void Mutate(float coef); // 0 to infinity : defines how strong the gene will change
    }
}
