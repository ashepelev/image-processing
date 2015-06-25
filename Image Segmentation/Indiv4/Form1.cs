using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Indiv4
{
    public partial class Form1 : Form
    {

        string filename;
        Image im;
        Bitmap btp, orig;

        int width, height, threshold = 10;
        bool loaded;

        struct Point
        {
            public Color c;
            public int x;
            public int y;
            public bool belongs;
            public int regNum;
            public bool border;
        };

        struct myRegion
        {
            public List<Point> regPoints;
            public List<Point> regBorder;
           
        };

        Point[,] newIm;
        List<Point> seeds = new List<Point>();
        List<myRegion> regs = new List<myRegion>();
        List<Color> avgcolors = new List<Color>();
        Color[] colors = {Color.Black,Color.Red,Color.Green,Color.Blue,Color.Brown,Color.Cyan,Color.Gold,Color.Orange};

        public Form1()
        {
            InitializeComponent();
            loaded = false;
            button2.Enabled = false;
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 255;
        }

        byte[] toGrayScale(Bitmap im)
        {
            byte[] result = new byte[im.Width*im.Height];
            newIm = new Point[width,height];
            int iter = 0;
            for (int i = 0; i < im.Width; ++i)
                for (int j = 0; j < im.Height; ++j)
                {
                    Color input = orig.GetPixel(i,j);
                    Point p = new Point();
                    p.c = input;
                    p.belongs = false;
                    p.x = i;
                    p.y = j;
                    p.regNum = -1;
                    newIm[i, j] = p;
                    result[iter] = (byte) (input.R*0.2125 + input.G*0.7154 + input.B*0.0721);
                    ++iter;
                }
            return result;
        }

        int otsuThreshold(byte[] image, int size)
        {
           int min=image[0], max=image[0];
           int i, temp, temp1;
           int[] hist;
           int histSize;
 
           int alpha, beta, threshold=0;
           double sigma, maxSigma=-1;
           double w1,a;
 
        // Построение гистограммы 
        // Узнаем наибольший и наименьший полутон
           for(i=1;i<size;i++)
           {
              temp=image[i];
              if(temp<min)   min = temp;
              if(temp>max)   max = temp;
           }
 
           histSize=max-min+1;
           hist = new int[histSize];
        //   if((hist=(int*) malloc(sizeof(int)*histSize))==NULL) return -1;
 
           for(i=0;i<histSize;i++)
              hist[i]=0;
 
        // Считаем сколько каких полутонов 
           for(i=0;i<size;i++)
              hist[image[i]-min]++;
 
        // Гистограмма построена 
 
           temp=temp1=0;
           alpha=beta=0;
        // Для расчета математического ожидания первого класса 
           for(i=0;i<=(max-min);i++)
           {
              temp += i*hist[i];
              temp1 += hist[i];
           }
 
        // Основной цикл поиска порога
        //Пробегаемся по всем полутонам для поиска такого, при котором внутриклассовая дисперсия минимальна 
           for(i=0;i<(max-min);i++)
           {
              alpha+= i*hist[i];
              beta+=hist[i];
 
              w1 = (double)beta / temp1;
              a = (double)alpha / beta - (double)(temp - alpha) / (temp1 - beta);
              sigma=w1*(1-w1)*a*a;
 
              if(sigma>maxSigma)
              {
                 maxSigma=sigma;
                 threshold=i;
              }
           }
           return threshold + min;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                filename = openFileDialog1.FileName;
                im = new Bitmap(filename);
                orig = (Bitmap)im.Clone();
                btp = (Bitmap)orig;
                loaded = true;
                button2.Enabled = true;
                pictureBox1.Image = im;
                
                width = pictureBox1.Image.Width;
                height = pictureBox1.Image.Height;
            }
        }

        private void Algo(int threshold, int regNum = 0)
        {
            Queue<Point> regPoints = new Queue<Point>();
            Queue<Point> borderPoints = new Queue<Point>();
            for (int i = 0; i < seeds.Count; ++i)
            {
                Point p = new Point();
                p.x = seeds[i].x;
                p.y = seeds[i].y;
                p.regNum = regNum;
                myRegion myr = new myRegion();
                myr.regBorder = new List<Point>();
                myr.regPoints = new List<Point>();
                avgcolors.Add(newIm[p.x,p.y].c);
                myr.regPoints.Add(p);
                regPoints.Enqueue(p);
                regs.Add(myr);
                MainAlgo(regPoints, borderPoints, threshold, i,true);
            }/*
            Point p = new Point();
            p.x = width / 2;
            p.y = height / 2;
            p.regNum = regNum;
            myRegion myr = new myRegion();
            myr.regBorder = new List<Point>();
            myr.regPoints = new List<Point>();
            myr.regPoints.Add(p);
      //      avgcolors.Add(newIm[p.x, p.y].c);
            regPoints.Enqueue(p);
            regs.Add(myr);
            MainAlgo(regPoints, borderPoints, threshold, regNum,true);
            while (borderPoints.Count > 0)
            {
                Point p1 = borderPoints.Dequeue();
          //      avgcolors.Add(newIm[p1.x, p1.y].c);
                myRegion myr1 = new myRegion();
                myr1.regBorder = new List<Point>();
                myr1.regPoints = new List<Point>();
                myr1.regPoints.Add(p1);
                regPoints.Clear();
                regPoints.Enqueue(p1);
                regs.Add(myr1);
                ++regNum;
                MainAlgo(regPoints, borderPoints, threshold, regNum,false);
            }*/
           
        }

        private void MainAlgo(Queue<Point> regPoints, Queue<Point> borderPoints, int threshold, int regNum, bool first)
        {
            while (regPoints.Count > 0)
            {
                Point p = regPoints.Dequeue();
                int x = p.x;
                int y = p.y;
                if (first && newIm[x, y].border)
                    continue;
                for (int i = x - 1; i <= x + 1; ++i)
                    for (int j = y - 1; j <= y + 1; ++j)
                    {
                        if (i < 0 || i > im.Width-1 || j < 0 || j > im.Height-1)
                            continue;
                        if (i == x && j == y)
                            continue;
                        if (newIm[i, j].regNum != -1)
                            continue;
                        if (newIm[i, j].border)
                            continue;
                        if (Distance(newIm[x, y].c, newIm[i, j].c) < threshold)
                    //    if (myDistance(newIm[i, j].c, regNum) < threshold)
                        {
                            newIm[i, j].regNum = regNum;
                            regs[regNum].regPoints.Add(newIm[i, j]);
                            regPoints.Enqueue(newIm[i, j]);
                            /*
                            Color avgc = avgcolors[regNum];
                            int avgr = avgc.R;
                            int avgb = avgc.B;
                            int avgg = avgc.G;
                            
                            avgr *= (regs[regNum].regPoints.Count - 1);
                            avgr += newIm[i, j].c.R;
                            avgr /= regs[regNum].regPoints.Count;

                            avgb *= (regs[regNum].regPoints.Count - 1);
                            avgb += newIm[i, j].c.B;
                            avgb /= regs[regNum].regPoints.Count;

                            avgg *= (regs[regNum].regPoints.Count - 1);
                            avgg += newIm[i, j].c.G;
                            avgg /= regs[regNum].regPoints.Count;

                          //  avgc = Color.FromArgb(255, avgr, avgg, avgb);
                            avgcolors[regNum] = Color.FromArgb(255, avgr, avgg, avgb);*/
                        }
                        else
                        {
                            regs[regNum].regBorder.Add(newIm[i, j]);
                            
                            borderPoints.Enqueue(newIm[i, j]);
                            newIm[i, j].border = true;
                        }
                    }
            }
        }

        private Bitmap makeImage()
        {
            Bitmap result = new Bitmap(width,height);
            double k = 255.0 / regs.Count;
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    result.SetPixel(i, j, Color.White);
            Random rnd = new Random();
            for (int i = 0; i < regs.Count; ++i)
            {
                if (regs[i].regPoints.Count < 2)
                {
                    break;
                }
                Color c = Color.FromArgb(255, rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
                for (int j = 0; j < regs[i].regPoints.Count; ++j)
                {
                    
                    result.SetPixel(regs[i].regPoints[j].x, regs[i].regPoints[j].y, c);// Color.FromArgb(255,255-(int)k*i,255-(int)k*i,255-(int)k*i));
                }
            }
            return result;
        }

        private double Distance(Color c, Color c1)
        {
            int dr = Math.Abs((c.R - c1.R) * (c.R - c1.R));
            int dg = Math.Abs((c.G - c1.G) * (c.G - c1.G));
            int db = Math.Abs((c.B - c1.B) * (c.B - c1.B));
            double dist = Math.Sqrt(dr + dg + db);
            return dist;
        }

        private double myDistance(Color c, int regNum)
        {
            Color c1 = avgcolors[regNum];
            int dr = Math.Abs((c.R - c1.R) * (c.R - c1.R));
            int dg = Math.Abs((c.G - c1.G) * (c.G - c1.G));
            int db = Math.Abs((c.B - c1.B) * (c.B - c1.B));
            double dist = Math.Sqrt(dr + dg + db);
            return dist;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = new Point();
            p.x = e.X;
            p.y = e.Y;
            p.c = btp.GetPixel(e.X, e.Y);
            seeds.Add(p);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.FillEllipse(Brushes.White, new Rectangle(e.X - 3, e.Y - 3, 7, 7));
            pictureBox1.Update();
            g.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            byte[] grayIm = toGrayScale((Bitmap)orig);
          //  threshold = otsuThreshold(grayIm, grayIm.Length);
            Algo(threshold);
            Bitmap mybtp = makeImage();
            pictureBox1.Image = (Image)mybtp;
            pictureBox1.Update();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            threshold = trackBar1.Value;
            label1.Text = String.Format("Threshold = {0}", threshold);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = (Image) orig.Clone();
            pictureBox1.Update();
            seeds.Clear();
            regs.Clear();
        }
    }
}
