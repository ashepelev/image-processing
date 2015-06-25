using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageProc3
{
    public class Digit
    {
        
        const int MATRIX_SIZE = 100;

        public int digit;
        public int center;
        public int centerv;

        public byte[,] matr;

        public Digit(int d, int center,int centerv)
        {
            if (d < 0 || d > 9)
                throw new Exception("Wrong digit: [0,9]");
            if (center < -MATRIX_SIZE + 1 || center > MATRIX_SIZE - 1)
                throw new Exception(String.Format("Wrong center: [-{0},{0}]", MATRIX_SIZE));
            this.center = center;
            matr = new byte[MATRIX_SIZE,MATRIX_SIZE];
            Image mydigit;
            Bitmap btp = new Bitmap(MATRIX_SIZE, MATRIX_SIZE);
            mydigit = btp;
            Graphics g = Graphics.FromImage(mydigit);
      //      System.Drawing.Font myf = new System.Drawing.Font(FontFamily.GenericSerif, 100);
            g.DrawString(d.ToString(), Form1.myFont, Brushes.Black, new RectangleF(MATRIX_SIZE / 10 + center, -MATRIX_SIZE / 10+centerv, MATRIX_SIZE + MATRIX_SIZE / 5, MATRIX_SIZE + MATRIX_SIZE / 2));
            btp = (Bitmap)mydigit;
            Color pixelColor;
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    pixelColor = btp.GetPixel(i, j);
                    if (pixelColor.A > 0)
                        matr[i, j] = 1;
                    else
                        matr[i, j] = 0;
                }
            g.Dispose();
            btp.Dispose();
            mydigit.Dispose();
        }
    }
}
