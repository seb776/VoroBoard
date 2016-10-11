using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvingKeyboard
{
    interface ISamplesSource
    {
        void NextSample();
        string GetCurrentSample();
    }
}
