using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProc2
{
    public class Triangle
    {
        const int MATRIX_SIZE = 100;

        public int type; // 0 - Down, 1 - Up
        public int[,] matr = new int[MATRIX_SIZE, MATRIX_SIZE];
        public int center;

        public Triangle(int t, int center)
        {
            center += MATRIX_SIZE/2;
            this.center = center;
            if (t < 0 || t > 1)
                throw new Exception("Wrong triangle type");
            if (center < -MATRIX_SIZE + 1 || center > MATRIX_SIZE - 1)
                throw new Exception(String.Format("Wrong center: [-{0},{0}]", MATRIX_SIZE));
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                    matr[i, j] = 0;
            if (t == 1)
            {
                for (int i = 0; i < MATRIX_SIZE; ++i)
                    for (int j = (int)(center - (double)(MATRIX_SIZE / (MATRIX_SIZE * 2.0)) * i); j <= (int)(center + (double)(MATRIX_SIZE / (MATRIX_SIZE * 2.0)) * i); ++j)
                        if (j < 0 || j >= MATRIX_SIZE)
                            continue;
                        else
                            matr[i, j] = 1;
            }
            else if (t == 0)
            {
                for (int i = 0; i < MATRIX_SIZE; ++i)
                    for (int j = (int)(center - (double)(MATRIX_SIZE / (MATRIX_SIZE * 2.0)) * (MATRIX_SIZE - i - 1)); j <= (int)(center + (double)(MATRIX_SIZE / (MATRIX_SIZE * 2.0)) * (MATRIX_SIZE - i - 1)); ++j)
                        if (j < 0 || j >= MATRIX_SIZE)
                            continue;
                        else
                            matr[i, j] = 1;
            }

        }
    }
}
