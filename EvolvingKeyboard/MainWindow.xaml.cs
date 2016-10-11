using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EvolvingKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isEvolving;
        private ISamplesSource _samplesSource = null;
        int intoSentence = 0;
        Random rr = new Random();
        String[] sentences;
        List<Letter> Letters;
        WriteableBitmap _wb;
        String lastLetter = null;
        List<Point> savedList;
        double lastScore = 0.0;
        double currentScore = 0.0;
        double temperature = 150.0;

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

        public MainWindow()
        {
            InitializeComponent();
            _wb = new WriteableBitmap(1280, 600, 96, 96, PixelFormats.Bgra32, null);
            this.Img.Source = _wb;
            String[] keyPlacement = {
            "1234567890","azertyuiop","qsdfghjklm","wxcvbn,;:!"," "
                                    };
            Letters = new List<Letter>();
            Random r = new Random();
            Point p = new Point(0, 0);
            int yStep = _wb.PixelHeight / keyPlacement.Length;
            foreach (var s in keyPlacement)
            {
                int xStep = _wb.PixelWidth / s.Length;
                p.X = 0;
                foreach (var l in s)
                {
                    Letters.Add(new Letter(new Point(p.X, p.Y), (uint)r.Next(0, Int32.MaxValue), this.KeyboardGrid, l.ToString()));
                    p.X += xStep;
                }
                p.Y += yStep;
            }

            this.Topmost = true;

            //drawKeyboard();

            _samplesSource = new DefaultLocalSamplesSource();

            ApplyNeighbors(GenerateNeighbors());
            drawKeyboard();
            this.DemoField.Content = _samplesSource.GetCurrentSample();
            this.Temperature.Content = temperature.ToString();
        }

        #region Events
        // TODO consider letter positions as UV (0-1 coordinates) for a more flexible approach
        // TODO Draw voronoi cells using WPF polys, it will be easier to handle keypress and will avoid heavy loops at each click
        private void KeyboardGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _simulateKeyPress('a');
            return;

            // We first get the letter we typed
            Point p = e.GetPosition(this.KeyboardGrid);
            Letter nearest = null;
            double distSqr = 0.0;
            foreach (var l in Letters)
            {
                double distCur = Math.Pow(p.X - l.Position.X, 2.0) + Math.Pow(p.Y - l.Position.Y, 2.0);
                if (nearest == null || distCur < distSqr)
                {
                    nearest = l;
                    distSqr = distCur;
                }
            }
            if (nearest != null)
            {
                lastLetter = (String)nearest.lbl.Content;
            }

            if (_isEvolving)
            {
                _doEvolution(lastLetter[0]);
            }
            else
            {
                _simulateKeyPress(lastLetter[0]);
            }

            currentScore += GetScore(lastLetter, _samplesSource.GetCurrentSample()[intoSentence].ToString());
            this.CurScore.Content = currentScore.ToString();
            ++intoSentence;
            if (intoSentence >= _samplesSource.GetCurrentSample().Length)
            {
                intoSentence = 0;
                this.DemoField.Content = _samplesSource.GetCurrentSample();
                this.TextField.Content = "";


                //drawKeyboard();
                if (!best)
                {
                    temperature *= 0.99;
                    this.Temperature.Content = temperature.ToString();

                    if (currentScore < lastScore || rr.NextDouble() < Math.Exp(-(currentScore - lastScore) / temperature))
                    {
                        lastScore = currentScore;
                        savedList = SaveCurrentSolution();
                    }
                    ApplyNeighbors(GenerateNeighbors());
                    drawKeyboard();
                }
            }
        }
        bool best = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            best = !best;

            if (best)
            {
                this.Switchbtn.Content = "Best locked";
                this.Topmost = true;
            }
            else
            {
                this.Switchbtn.Content = "Evolving";
                this.Topmost = false;
            }
        }
        #endregion Events


        #region KeyBoard display
        public void drawKeyboard()
        {
            for (int y = 0; y < _wb.PixelHeight; ++y)
            {
                for (int x = 0; x < _wb.PixelWidth; ++x)
                {
                    Letter nearest = null;
                    double distSqr = 0.0;
                    foreach (var l in Letters)
                    {
                        double distCur = Math.Pow(x - l.Position.X, 2.0) + Math.Pow(y - l.Position.Y, 2.0);
                        if (nearest == null || distCur < distSqr)
                        {
                            nearest = l;
                            distSqr = distCur;
                        }
                    }
                    if (nearest != null)
                    {
                        WritePixel(x, y, nearest.Color);
                    }
                }
            }
        }
        public void WritePixel(int x, int y, uint color)
        {
            uint[] array = 
            {
                color
            };
            _wb.WritePixels(new Int32Rect(x, y, 1, 1), array, (_wb.Format.BitsPerPixel + 7) / 8, 0);
        }
        #endregion KeyBoard display


        #region Learning algorithm
        private void _doEvolution(char c)
        {
            this.TextField.Content += c.ToString();

        }
        public List<Point> GenerateNeighbors()
        {
            double percentMove = 0.1;
            var newlist = new List<Point>();
            for (int i = 0; i < Letters.Count; ++i)
            {
                newlist.Add(new Point(Letters[i].Position.X + ((rr.NextDouble() - 0.5) * percentMove * Letters[i].Position.X),
                    Letters[i].Position.Y + ((rr.NextDouble() - 0.5) * percentMove * Letters[i].Position.Y)));
            }
            return newlist;
        }

        public void ApplyNeighbors(List<Point> pts)
        {
            for (int i = 0; i < pts.Count; ++i)
            {
                Letters[i].Position = pts[i];
                Letters[i].lbl.Margin = new Thickness(pts[i].X, pts[i].Y, 0, 0);
            }
        }

        public List<Point> SaveCurrentSolution()
        {
            List<Point> list = new List<Point>();

            for (int i = 0; i < Letters.Count; ++i)
            {
                list.Add(new Point(Letters[i].Position.X, Letters[i].Position.Y));
            }
            return list;
        }

        public double GetScore(String typed, String should)
        {
            Letter ty = null;
            Letter sh = null;
            foreach (var l in Letters)
            {
                if (l.lbl.Content.Equals(typed))
                    ty = l;
                if (l.lbl.Content.Equals(should))
                    sh = l;
            }
            return Math.Pow(ty.Position.X - sh.Position.X, 2.0) + Math.Pow(ty.Position.Y - sh.Position.Y, 2.0);
        }
        #endregion Learning algorithm

        #region KeyBoard simulation
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }
        private void _simulateKeyPress(char c)
        {

            // There should be a way of defining the receiving window
            WindowsInput.InputSimulator s = new WindowsInput.InputSimulator();
            //this.WindowState = System.Windows.WindowState.Minimized;
            //System.Threading.Thread.Sleep(50);
            WindowsInput.KeyboardSimulator k = new WindowsInput.KeyboardSimulator(s);
            s.Keyboard.TextEntry(lastLetter + "totototo");
            k.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_A);
            //System.Threading.Thread.Sleep(50);
            //this.WindowState = System.Windows.WindowState.Normal;

            return;


            System.Windows.Forms.SendKeys.SendWait(lastLetter);


        }
        #endregion KeyBoard simulation
    }
}