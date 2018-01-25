using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvingKeyboard.Keyboard
{
    public class Vertex : IVertex
    {
        public Vertex(double x, double y)
        {
            Position = new double[] {
                x, y
            };
        }
        public double[] Position
        {
            get;
            set;
        }
    }
}
