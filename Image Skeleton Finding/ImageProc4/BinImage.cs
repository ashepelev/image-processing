using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProc4
{
    public class BinImage
    {
        public byte[,] matr;
        private byte[,] mask;

        public int Width;
        public int Height;
        public int MaskSize;

        public BinImage(byte[,] matr, int w, int h)
        {
            this.matr = matr;
            this.Width = w;
            this.Height = h;
        }

        public void LoadMask(byte[,] mask)
        {
            this.mask = mask;
            this.MaskSize = (int)Math.Sqrt(mask.Length);
        }


        public void Dilation()
        {
            byte[,] matr1 = new byte[Width, Height];
            matr1 = (byte[,])matr.Clone();
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (matr[i, j] == 1)
                        for (int k = 0; k < MaskSize; ++k)
                            for (int l = 0; l < MaskSize; ++l)
                                if (mask[k, l] == 1)
                                {
                                    int ii = (i - MaskSize / 2) + k;
                                    int jj = (j - MaskSize / 2) + l;
                                    if (!((ii < 0) || (ii >= Width)))
                                        if (!((jj < 0) || (jj >= Height)))
                                            matr1[(i - MaskSize / 2) + k, (j - MaskSize / 2) + l] = 1;
                                }
            this.matr = matr1;
        }

        public void Erosion()
        {
            byte[,] matr1 = new byte[Width, Height];
            matr1 = (byte[,])matr.Clone();
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                {
                    if (matr[i, j] == 1)
                        for (int k = 0; k < MaskSize; ++k)
                            for (int l = 0; l < MaskSize; ++l)
                                if (mask[k, l] == 1)
                                {
                                    int ii = (i - MaskSize / 2) + k;
                                    int jj = (j - MaskSize / 2) + l;
                                    if (!((ii < 0) || (ii >= Width)))
                                        if (!((jj < 0) || (jj + l >= Height)))
                                            if (matr[ii, jj] != 1)
                                            {
                                                matr1[i, j] = 0;
                                                goto NextPixel;
                                            }
                                }
                NextPixel:
                    continue;
                }
            this.matr = matr1;
        }




    }
}
