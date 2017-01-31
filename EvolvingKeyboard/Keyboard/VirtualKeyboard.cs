using EvolvingKeyboard.StochasticEvolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EvolvingKeyboard.Keyboard
{
    public class VirtualKeyboard : Image
    {
        public Individual<Letter> Individual; // the DNA of the keyboard

        private Action<Letter, Point> _onClickCallback;
        private Letter[,] _lettersOnImage; // To make the 
        private WriteableBitmap _bitmap;


        private VirtualKeyboard() { }
        public VirtualKeyboard(int widthPx, int heightpX, string[] letters, Action<Letter, Point> onClick)
        {
            this.MouseLeftButtonDown += VirtualKeyboard_MouseLeftButtonDown;
            this.UpdateKeyboard(widthPx, heightpX, letters, onClick);
        }

        void VirtualKeyboard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Letter receivedLetter = null;
            throw new NotImplementedException();
            _onClickCallback.Invoke(receivedLetter, e.GetPosition(this));
        }

        #region Updates
        /// <summary>
        /// Changes the keyboard to desired parameters.
        /// </summary>
        /// <param name="widthPx"></param>
        /// <param name="heightpX"></param>
        /// <param name="letters"></param>
        /// <param name="onClick"></param>
        public void UpdateKeyboard(int widthPx, int heightpX, string[] letters, Action<Letter, Point> onClick)
        {
            _onClickCallback = onClick;
            _bitmap = new WriteableBitmap(widthPx, heightpX, 96, 96, PixelFormats.Bgra32, null);
            this.Source = _bitmap;
            List<Letter> lettersList = new List<Letter>();
            foreach (var str in letters)
            {
                foreach (var key in str)
                {
                    lettersList.Add(new Letter(new Point(), 0U, null, key.ToString()));
                }
            }
            Individual = new Individual<Letter>(lettersList);
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
            UpdateKeyboard();
        }

        /// <summary>
        /// Updated the displayed keyboard.
        /// </summary>
        public void UpdateKeyboard()
        {

        }
        #endregion Updates
    }
}
