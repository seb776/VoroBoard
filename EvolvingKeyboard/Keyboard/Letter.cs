using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EvolvingKeyboard.Keyboard
{
    public class Letter
    {
        public Letter(Point p, uint color, Grid g, String l)
        {
            Color = color;
            lbl = new Label();
            lbl.HorizontalAlignment = HorizontalAlignment.Left;
            lbl.VerticalAlignment = VerticalAlignment.Top;
            lbl.Content = l;
            Position = p;
            g.Children.Add(lbl);
        }
        Point _pt;
        public Point Position
        {
            get
            {
                return _pt;
            }
            set
            {
                _pt = value;
                lbl.Margin = new Thickness(_pt.X, _pt.Y, 0, 0);
            }
        }
        public uint Color;
        public Label lbl;
    }

}
