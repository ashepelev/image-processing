using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProc4
{
    public partial class Form1 : Form
    {
        public static Font myFont = new Font("Orator Std", 75.0f);
        Bitmap btp, btp1;
        Image im;
        byte[,] matr = new byte[100, 100];
        public const int MATRIX_SIZE = 100;

        struct Point
        {
            public int x;
            public int y;
        }

        public Form1()
        {
            InitializeComponent();
        }

        public void test()
        {
            btp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp1;
            pictureBox1.BackColor = Color.White;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            //   Font 
            //       System.Drawing.Font myf = new Font("Buxton Sketch", 100.0f);
            g.DrawString("3", myFont, Brushes.Black, new RectangleF(-MATRIX_SIZE / 10, -MATRIX_SIZE / 10, MATRIX_SIZE + MATRIX_SIZE / 5, MATRIX_SIZE + MATRIX_SIZE / 2));
            pictureBox1.Update();
            //        g.DrawString(
        }

        private void DrawMatrix(BinImage bi, PictureBox p)
        {
            //      pictureBox1.Dispose();
            btp = new Bitmap(bi.Width, bi.Height);
            for (int i = 0; i < bi.Width; ++i)
                for (int j = 0; j < bi.Height; ++j)
                    if (bi.matr[i, j] == 1)
                        btp.SetPixel(i, j, Color.White);
                    else
                        btp.SetPixel(i, j, Color.Black);
            p.Image = btp;
            p.Invalidate();
            p.Update();
        }

        private byte[,] loadMatrix(Digit d)
        {
            btp = (Bitmap)im;
            Color pixelColor;
            byte[,] result = new byte[MATRIX_SIZE, MATRIX_SIZE];
            for (int i = 0; i < im.Width; ++i)
                for (int j = 0; j < im.Height; ++j)
                {
                    pixelColor = btp.GetPixel(i, j);
                    if (pixelColor.A > 0 && pixelColor.R > 127 && pixelColor.G > 127 && pixelColor.B > 127)
                       result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            return result;
        }

        private Digit generateDigit()
        {
            Random rnd = new Random();
            Digit d = new Digit(rnd.Next(0, 10), 0, 0);
            return d;
        }

        private bool checkImage(byte[,] t, int fullpoints)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    if (t[i, j] == 0)
                    {
                        Point p;
                        p.x = i;
                        p.y = j;
                        points.Add(p);
                    }                        
                }
            if (points.Count < MATRIX_SIZE * MATRIX_SIZE * (fullpoints / 10) / 2000)
                return false;
         /*   int sumx = 0, sumy = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                sumx += points[i].x;
                sumy += points[i].y;
            }
            sumx /= points.Count;
            sumy /= points.Count;
            if (sumx < 40 || sumx > 60)
                return false;
            if (sumy < 40 || sumy > 60)
                return false;*/
            return true;
        }

        private byte[,] createCircleMatr(int circleDiam)
        {
            Image circle;
            Bitmap bcircle = new Bitmap(circleDiam, circleDiam);
            circle = bcircle;
            Graphics gr = Graphics.FromImage(circle);
            gr.FillEllipse(Brushes.White, 0, 0, circleDiam, circleDiam);
            byte[,] mask = new byte[circleDiam, circleDiam];
            bcircle = (Bitmap)circle;
            Color pixelColor;
            for (int i = 0; i < circleDiam; ++i)
                for (int j = 0; j < circleDiam; ++j)
                {
                    pixelColor = bcircle.GetPixel(i, j);
                    if (pixelColor.A > 0 && pixelColor.R > 127 && pixelColor.G > 127 && pixelColor.B > 127)
                        mask[i, j] = 1;
                    else
                        mask[i, j] = 0;
                }
            return mask;
        }

        private int goodPointsCount(byte[,] t)
        {
            int result = 0;
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    if (t[i, j] == 0)
                    {
                        ++result;
                    }
                }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Digit d = generateDigit();
            BinImage bi = new BinImage((byte[,])d.matr.Clone(), MATRIX_SIZE, MATRIX_SIZE);
            byte[,] copy = (byte[,])bi.matr.Clone();
            int maskSize = 0;            
            DrawMatrix(bi, pictureBox1);
            int fullpoints = goodPointsCount(bi.matr);
            while (checkImage(bi.matr,fullpoints))
            {
                maskSize += 1;
                bi.matr = (byte[,])copy.Clone();
                byte[,] ball = createCircleMatr(maskSize);
                bi.LoadMask(ball);
                bi.Dilation();
                DrawMatrix(bi, pictureBox2);
            }
            maskSize += 1;
            bi.MaskSize--;
            bi.matr = (byte[,])copy.Clone();
            byte[,] ball1 = createCircleMatr(maskSize);
            bi.LoadMask(ball1); ;
            bi.Dilation();
            DrawMatrix(bi, pictureBox2);
        }

    }
}
