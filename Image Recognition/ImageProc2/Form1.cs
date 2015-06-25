using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProc2
{
    public partial class Form1 : Form
    {
        const int MATRIX_SIZE = 100;
        const double h = 0.01;
        const int SAMPLE_COUNT = 50;
        const int ELEMS = MATRIX_SIZE * MATRIX_SIZE;

        Bitmap btp1, btp2;

        Triangle[] upper = new Triangle[SAMPLE_COUNT];
        Triangle[] downer = new Triangle[SAMPLE_COUNT];

        List<string> lines = new List<string>();

        double[] solveL;
        double solveTetta;

        public Form1()
        {
            InitializeComponent();
            init();
            loadSamples();
            //     drawTriangles(-2, 5);
            algo();
            drawTriangles(upper[0], downer[0]);
        }

        public double[] createW(Triangle t1, Triangle t2)
        {
            double[] result = new double[ELEMS];
            for (int i = 0; i < ELEMS; ++i)
                result[i] = t1.matr[i / MATRIX_SIZE, i % MATRIX_SIZE] - t2.matr[i / MATRIX_SIZE, i % MATRIX_SIZE];
            return result;
        }

        public void algo()
        {
       //     List<string> lines = new List<string>();
            lines.Add("Starting...");
            richTextBox1.Lines = lines.ToArray();
            double minw = int.MaxValue;
            int minwnum = 0;
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                double[] W = createW(upper[i],downer[i]);
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
            double[] L = createW(upper[minwnum], downer[minwnum]);
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
                double[] W = createW(upper[i],downer[i]);
                for (int j = 0; j < ELEMS; ++j)
                    sum += W[j] * W[j] - L[j] * W[j];
                lambda = sum;
                sum = 0;
                for (int j = 0; j < ELEMS; ++j)
                    sum += L[j]*L[j] - 2*L[j]*W[j] + W[j]*W[j];
                lambda /= sum;
                if (i == 0)
                    prevlambda = lambda;
                lines.Add(String.Format("Iter {0}: Lambda = {1}", iternum, lambda));
                richTextBox1.Lines = lines.ToArray();
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
            solveL = finalL;
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
                    prodx += finalL[j] * upper[i].matr[j/MATRIX_SIZE,j%MATRIX_SIZE];
                    prody += finalL[j] * downer[i].matr[j / MATRIX_SIZE, j % MATRIX_SIZE];
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
                solveTetta = (mincosx + maxcosy) / 2.0;
                lines.Add(String.Format("Tetta: {0}", solveTetta ));
                richTextBox1.Lines = lines.ToArray();
        }


        public void init()
        {
            btp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp1;
            pictureBox1.BackColor = Color.White;
            btp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox2.Image = btp2;
            pictureBox2.BackColor = Color.White;
        }

        public void loadSamples()
        {
            Random rnd = new Random();
            for (int i = 0; i < SAMPLE_COUNT; ++i)
            {
                int c1 = rnd.Next(-MATRIX_SIZE / 2, MATRIX_SIZE / 2);
                int c2 = rnd.Next(-MATRIX_SIZE / 2, MATRIX_SIZE / 2);
                upper[i] = new Triangle(1, c1);
                downer[i] = new Triangle(0, c2);

            }
        }


        public void drawTriangles(int center1, int center2)
        {
            init();
            int ELEMS = MATRIX_SIZE * MATRIX_SIZE;
            Graphics g1 = Graphics.FromImage(pictureBox1.Image);
            Graphics g2 = Graphics.FromImage(pictureBox2.Image);
            Point[] p1 = { new Point(center1 * MATRIX_SIZE, 0), new Point((center1 - 5) * MATRIX_SIZE, ELEMS), new Point((center1 + 5) * MATRIX_SIZE, ELEMS) };
            Point[] p2 = { new Point(center2 * MATRIX_SIZE, ELEMS), new Point((center2 - 5) * MATRIX_SIZE, 0), new Point((center2 + 5) * MATRIX_SIZE, 0) };
            g1.FillPolygon(Brushes.Black, p1);
            g2.FillPolygon(Brushes.Black, p2);
            pictureBox1.Update();
            pictureBox2.Update();
        }

        public void drawTriangles(Triangle tr1, Triangle tr2)
        {
            init();
            Bitmap p1 = new Bitmap(MATRIX_SIZE, MATRIX_SIZE);
            Bitmap p2 = new Bitmap(MATRIX_SIZE, MATRIX_SIZE);
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    if (tr1.matr[i, j] == 0)
                        p1.SetPixel(i, j, Color.White);
                    else
                        p1.SetPixel(i, j, Color.Black);
                    if (tr2.matr[i, j] == 0)
                        p2.SetPixel(i, j, Color.White);
                    else
                        p2.SetPixel(i, j, Color.Black);
                }
            pictureBox1.Image = p1;
            pictureBox2.Image = p2;
            pictureBox1.Update();
            pictureBox2.Update();
        }

        public void drawSingleTriangle(Triangle tr,PictureBox p)
        {
            init();
            Bitmap p1 = new Bitmap(MATRIX_SIZE, MATRIX_SIZE);
            for (int i = 0; i < MATRIX_SIZE; ++i)
                for (int j = 0; j < MATRIX_SIZE; ++j)
                {
                    if (tr.matr[i, j] == 0)
                        p1.SetPixel(i, j, Color.White);
                    else
                        p1.SetPixel(i, j, Color.Black);
                }
            p.Image = p1;
            p.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Triangle tr = new Triangle(rnd.Next(0,2),rnd.Next(-MATRIX_SIZE / 2, MATRIX_SIZE / 2));
            for (int i = 0; i < 10000; ++i)
            {
                tr.matr[rnd.Next(0,MATRIX_SIZE),rnd.Next(0,MATRIX_SIZE)] = 1;
            }
            drawSingleTriangle(tr, pictureBox3);
            double prodx = 0;
            for (int j = 0; j < ELEMS; ++j)
            {
                prodx += solveL[j] * tr.matr[j/MATRIX_SIZE,j%MATRIX_SIZE];
            }
            int recType = -1;
            if (prodx > solveTetta)
                recType = 1;
            else
                recType = 0;
           Triangle recTr = new Triangle(recType,0);
            drawSingleTriangle(recTr,pictureBox4);

            lines.Add(String.Format("L * T = {0}", prodx));
            richTextBox1.Lines = lines.ToArray();

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
