using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProc3
{
    public partial class Form1 : Form
    {
        public static Font myFont = new Font("Courant", 85.0f);
    //    public static Font myFont = new Font("Eurostile", 85.0f);
    //    public static Font myFont = new Font("ZnikomitNo24", 85.0f);
   //     public static Font myFont = new Font("Samba DecorC", 70.0f);
  //      public static Font myFont = new Font("Still Time Cyr", 70.0f);
  //      public static Font myFont = new Font("Dali", 70.0f);
    //    public static Font myFont = new Font("Tourist Trap", 60.0f);
   //     public static Font myFont = new Font("Shlapak Script", 120.0f);


        const int MATRIX_SIZE = 100;
        const double h = 0.01;
        const int SAMPLE_COUNT = 100;
        const int ELEMS = MATRIX_SIZE * MATRIX_SIZE;
        public static int border = 2;
        public static int vborder = 2;

        Digit[,] digits = new Digit[10, SAMPLE_COUNT];
        List<string> lines = new List<string>();
   //     double[,] solveL = new double[10,

        public struct solve
        {
            public double[] vector;
            public double Tetta;
            public double mincosx;
            public double maxcosx;
        }

  //      List<solve> solves = new List<solve>(10);
 //       solve[,] solves = new solve[10,10];
        solve[] solves = new solve[10];

        Bitmap btp1, btp2;

        public Form1()
        {
            InitializeComponent();
            
            complex_algo();
      //            test2();
       //     pictureBox1.Dispose();
       //     pictureBox2.Dispose();
            pictureBox1.Image = new Bitmap(100, 100);
            pictureBox2.Image = new Bitmap(100, 100);
            pictureBox1.BackColor = Color.White;
            pictureBox2.BackColor = Color.White; pictureBox1.Image = new Bitmap(100, 100);
            test();
        }

        public void test2()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("../../Solver2.txt", false);
            file.WriteLine("Test");
            file.Close();
        }

        public void test()
        {
            btp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp1;
            pictureBox1.BackColor = Color.White;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
         //   Font 
     //       System.Drawing.Font myf = new Font("Buxton Sketch", 100.0f);
            g.DrawString("3", myFont, Brushes.Black, new RectangleF(MATRIX_SIZE/10, -MATRIX_SIZE / 10, MATRIX_SIZE + MATRIX_SIZE / 5, MATRIX_SIZE + MATRIX_SIZE / 2));
            pictureBox1.Update();
        //        g.DrawString(
        }

        public void generateSamples()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < SAMPLE_COUNT; ++j)
                {
                    digits[i, j] = new Digit(i, rnd.Next(-border, border),rnd.Next(-vborder,vborder));
                }
        }

        public double[] createW(int d, int d2, int k)
        {
            double[] result = new double[9*ELEMS];
            for (int i = 0; i < ELEMS; ++i)
                result[i] = digits[d, k].matr[i / MATRIX_SIZE, i % MATRIX_SIZE] - digits[d2, k].matr[i / MATRIX_SIZE, i % MATRIX_SIZE];
            return result;
        }

        public void complex_algo()
        {
            generateSamples();
     //       readInfo();
  /*          for (int i = 0; i < 10; ++i)
                for (int j = 0; j < 10; ++j)
                {
                    if (j == i)
                        continue;
                    algo(i, j);
                }
*/
            for (int i = 0; i < 10; ++i)
                algo(i);
   //         writeInfo();
        }
        /*
                public void readInfo()
                {
                    System.IO.StreamReader file = new System.IO.StreamReader("../../Solver.txt");
                    for (int i = 0; i < 10; ++i)
                    {
                        double[] vector = new double[ELEMS];
                        for (int j = 0; j < ELEMS; ++j)
                        {
                            try
                            {
                                vector[j] = double.Parse(file.ReadLine());
                            }
                            catch (System.FormatException e)
                            {
                                vector[j] = int.Parse(file.ReadLine());
                            }
                        }
                        double tetta = double.Parse(file.ReadLine());
                        file.ReadLine(); // \n
                        solves[i].vector = vector;
                        solves[i].Tetta = tetta;
                    }
                    file.Close();
                }

                public void writeInfo()
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("../../Solver.txt", false);
                    for (int i = 0; i < 10; ++i)
                    {
                        String[] vector = new String[ELEMS];
                        //   String tetta;
                        for (int j = 0; j < ELEMS; ++j)
                        {
                            vector[j] = solves[i].vector[j].ToString();
                        }
                        foreach (string line in vector)
                        {
                                file.WriteLine(line);
                        }
                        file.WriteLine(solves[i].Tetta.ToString());
                        file.WriteLine("\n");
                    }
                    file.Close();
                }
        */
        /*
        public void algo(int digit, int digit2)
        {
            lines.Add("Starting...");
            richTextBox1.Lines = lines.ToArray();
            double minw = int.MaxValue;
            int minwnum = 0;
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                double[] W = createW(digit,digit2,i);
                double curw = 0;
                for (int j = 0; j < ELEMS; ++j)
                {
                    curw += W[j] * W[j];
                }
                curw = Math.Sqrt(curw);
                if (curw < minw)
                {
                    minw = curw;
                    minwnum = i;
                }
            }
            double[] L = createW(digit,digit2,minwnum);
            double[] finalL = L;
            double minL = minw;
            int minLiter = int.MaxValue;
            int iternum = 0;
            double prevlambda = int.MinValue;
            int lambdaeq = 0;
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                double lambda = 0;
                double sum = 0;
                double[] W = createW(digit,digit2,i);
                for (int j = 0; j < ELEMS; ++j)
                    sum += W[j] * W[j] - L[j] * W[j];
                lambda = sum;
                sum = 0;
                for (int j = 0; j < ELEMS; ++j)
                    sum += L[j] * L[j] - 2 * L[j] * W[j] + W[j] * W[j];
                lambda /= sum;
                if (i == 0)
                    prevlambda = lambda;
        //        lines.Add(String.Format("Iter {0}: Lambda = {1}", iternum, lambda));
       //         richTextBox1.Lines = lines.ToArray();
                ++iternum;
                if (lambda > 0 && lambda < 1)
                {
                    double curL = 0;
                    for (int j = 0; j < ELEMS; ++j)
                    {
                        L[j] = L[j] * lambda + (1 - lambda) * W[j];
                        curL += L[j] * L[j];
                    }
                    curL = Math.Sqrt(curL);
                    if (curL < minL)
                    {
                        minL = curL;
                        finalL = (double[])L.Clone();
                        minLiter = i;
                    }
                }
                if (lambda > 1)
                    ++lambdaeq;
                else
                    if (lambdaeq > 0)
                        lambdaeq = 0;
                if (lambdaeq == 5)
                {
                    lines.Add(String.Format("Lambda stabilized"));
                    richTextBox1.Lines = lines.ToArray();
                    break;
                }
                prevlambda = lambda;
            }
            if (lambdaeq < 5)
            {
                lines.Add(String.Format("Run out of samples"));
                richTextBox1.Lines = lines.ToArray();
                return;
            }
            solves[digit,digit2].vector = finalL;
            //     drawTriangles(upper[minLiter].center, downer[minLiter].center);
            lines.Add(String.Format("Minimal L length: {0} at iter {1}", minL, minLiter));
            lines.Add("\n");
            richTextBox1.Lines = lines.ToArray();
            double maxcosy = int.MinValue;
            double mincosx = int.MaxValue;
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                double prodx = 0, prody = 0;
                for (int j = 0; j < ELEMS; ++j)
                {
                    prodx += finalL[j] * digits[digit,i].matr[j / MATRIX_SIZE, j % MATRIX_SIZE];
                    prody += finalL[j] * digits[digit2,i].matr[j / MATRIX_SIZE, j % MATRIX_SIZE];
                }
                //     prodx /= minL;
                //     prody /= minL;
                if (prody > maxcosy)
                    maxcosy = prody;
                if (prodx < mincosx)
                    mincosx = prodx;

            }
            lines.Add(String.Format("Min cos X: {0}", mincosx));
            lines.Add(String.Format("Max cos Y: {0}", maxcosy));
            lines.Add("\n");
            solves[digit,digit2].Tetta = (mincosx + maxcosy) / 2.0;
            lines.Add(String.Format("Tetta: {0}", solves[digit, digit2].Tetta));
            richTextBox1.Lines = lines.ToArray();
        }
        */
        public void algo(int digit)
        {
            lines.Add(String.Format("Digit {0}. Start...", digit));
            richTextBox1.Lines = lines.ToArray();
            double minw = int.MaxValue;
            int mininum = 0;
            int minjnum = 0;
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < SAMPLE_COUNT; ++j)
                {
                    if (i == digit)
                        continue;
                    double[] W = createW(digit, i, j);
                    double curw = 0;
                    for (int k = 0; k < ELEMS; ++k)
                    {
                        curw += W[k] * W[k];
                    }
                    curw = Math.Sqrt(curw);
                    if (curw < minw)
                    {
                        minw = curw;
                        mininum = i;
                        minjnum = j;
                    }
                }
            double[] L = createW(digit, mininum, minjnum);
            double[] finalL = L;
            double minL = minw;
            int minLiter = int.MaxValue;
            int iternum = 0;
            double prevlambda = int.MinValue;
            int lambdaeq = 0;
            bool lambdastab = false;
            for (int k = 0; k < 10; ++k)
            {
                if (lambdastab)
                    break;
                if (k == digit)
                    continue;
                for (int i = 0; i < SAMPLE_COUNT; ++i)
                {
                    
                    double lambda = 0;
                    double sum = 0;
                    double[] W = createW(digit, k, i);
                    for (int j = 0; j < ELEMS; ++j)
                        sum += W[j] * W[j] - L[j] * W[j];
                    lambda = sum;
                    sum = 0;
                    for (int j = 0; j < ELEMS; ++j)
                        sum += L[j] * L[j] - 2 * L[j] * W[j] + W[j] * W[j];
                    lambda /= sum;
                    if (i == 0)
                        prevlambda = lambda;
                    //       lines.Add(String.Format("Iter {0}: Lambda = {1}", iternum, lambda));
                    //       richTextBox1.Lines = lines.ToArray();
                    ++iternum;
                    if (lambda > 0 && lambda < 1)
                    {
                        double curL = 0;
                        for (int j = 0; j < ELEMS; ++j)
                        {
                            L[j] = L[j] * lambda + (1 - lambda) * W[j];
                            curL += L[j] * L[j];
                        }
                        curL = Math.Sqrt(curL);
                        if (curL < minL)
                        {
                            minL = curL;
                            finalL = (double[])L.Clone();
                            minLiter = i;
                        }
                    }
                    if (lambda > 1)
                        ++lambdaeq;
                    else
                        if (lambdaeq > 0)
                            lambdaeq = 0;
         /*           if (lambdaeq == 5)
                    {
                        lines.Add(String.Format("Lambda stabilized"));
                        richTextBox1.Lines = lines.ToArray();
                        lambdastab = true;
                        break;
                    } */
                    prevlambda = lambda;
                }
            }
            /*       if (lambdaeq < 5)
                   {
                       lines.Add(String.Format("Run out of samples"));
                       richTextBox1.Lines = lines.ToArray();
                       return;
                   }*/
            //    solves[digit] = new solve();
            solves[digit].vector = finalL;
            //     drawTriangles(upper[minLiter].center, downer[minLiter].center);
            lines.Add(String.Format("Minimal L length: {0} at iter {1}", minL, minLiter));
            lines.Add("\n");
            richTextBox1.Lines = lines.ToArray();
            double maxcosy = int.MinValue;
            double mincosx = int.MaxValue;
            double maxcosx = int.MinValue;
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                double prodx = 0, prody = 0;
                int count = 0;
                for (int j = 0; j < ELEMS; ++j)
                {
                    prodx += finalL[j] * digits[digit, i].matr[j / MATRIX_SIZE, j % MATRIX_SIZE];

                }
                if (prodx < mincosx)
                    mincosx = prodx;
                for (int k = 0; k < 10; ++k)
                {
                    prody = 0;
                    if (k == digit)
                        continue;
                    for (int j = 0; j < ELEMS; ++j)
                        prody += finalL[j] * digits[k, i].matr[j / MATRIX_SIZE, j % MATRIX_SIZE];
                    if (prody > maxcosy)
                        maxcosy = prody;                    
                }

                //     prodx /= minL;
                //     prody /= minL;

            }
            lines.Add(String.Format("Min cos X: {0}", mincosx));
            lines.Add(String.Format("Max cos Y: {0}", maxcosy));
            lines.Add("\n");
            solves[digit].Tetta = (mincosx + maxcosy) / 2.0;
            solves[digit].maxcosx = maxcosx;
            solves[digit].mincosx = mincosx;
            lines.Add(String.Format("Tetta: {0}", solves[digit].Tetta));
            lines.Add("\n");
            lines.Add("\n");
            richTextBox1.Lines = lines.ToArray();
        }
        public void drawSingleDigit(Digit di, PictureBox p)
        {
         //   p.Dispose();
       //     pictureBox1.Image.Dispose();
         //   pictureBox1.Image = new Bitmap(100, 100);
            Bitmap p1 = new Bitmap(MATRIX_SIZE, MATRIX_SIZE);
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    if (di.matr[i, j] == 0)
                        p1.SetPixel(i, j, Color.White);
                    else
                        p1.SetPixel(i, j, Color.Black);
                }
            p.Image = p1;
    //        p.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Digit di = new Digit(rnd.Next(0,10),rnd.Next(-border, border),rnd.Next(-vborder,vborder));
    /*        for (int i = 0; i < 10000; ++i)
            {
                tr.matr[rnd.Next(0, MATRIX_SIZE), rnd.Next(0, MATRIX_SIZE)] = 1;
            }*/
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
            {
                    if (rnd.NextDouble() < 0.1)
                        di.matr[i, j] = (byte) ((di.matr[i, j] + 1) % 2);
            }
            drawSingleDigit(di, pictureBox1);
            double prodx = 0;
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < ELEMS; ++j)
                {
                    prodx += solves[i].vector[j] * di.matr[j / MATRIX_SIZE, j % MATRIX_SIZE];
                }
                if (prodx > solves[i].Tetta)
                {
                    lines.Add(String.Format("This is {0}", i));
                    Digit di2 = new Digit(i, 0, 0);
                    drawSingleDigit(di2, pictureBox2);
                    richTextBox1.Lines = lines.ToArray();
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
                prodx = 0;
            }
            lines.Add(String.Format("\n"));
            richTextBox1.Lines = lines.ToArray();
            /*
            int recType = -1;
            if (prodx > solveTetta)
                recType = 1;
            else
                recType = 0;
            Triangle recTr = new Triangle(recType, 0);
            drawSingleTriangle(recTr, pictureBox4);

            lines.Add(String.Format("L * T = {0}", prodx));
            richTextBox1.Lines = lines.ToArray();

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret(); */
        }

    }
}
