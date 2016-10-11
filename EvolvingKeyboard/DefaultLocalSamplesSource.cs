using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvingKeyboard
{
    class DefaultLocalSamplesSource : ISamplesSource
    {
        uint _currentSample = 0U;
        String[] _sentences = {
                           "deux approches",
                           "sont possibles",
                           "quant a la variation",
                           "de la temperature",
                           "le pseudocode",
                           "suivant met en place",
                           "le recuit simule",
                           "tel que decrit",
                           "plus haut",
                           "a chaque iteration",
                           "de l algorithme",
                           "une modification elementaire",
                           "de la solution est effectuee"
                       };
        public void NextSample()
        {
            _currentSample++;
            if (_currentSample >= _sentences.Count())
                _currentSample = 0U;
        }
        public string GetCurrentSample()
        {
            return _sentences[_currentSample];
        }
    }
}
