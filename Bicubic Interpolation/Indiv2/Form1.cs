using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Indiv2
{
    public partial class Form1 : Form
    {
        String filename;
        Image im;
        Bitmap btp;
        Bitmap orig;

        struct myColor
        {
            public int R;
            public int G;
            public int B;
        };

        

        int medianRadius = 1;
        int maxFilterRadius = 1;
        double zoomk = 2.0;

        bool mousedown = false;
        bool allowdraw = false;
        bool goactive = true;

        int x, y, x1, y1, width, height;

        Color[,] input;
        Color[,] output;

        // Медианный фильтр + выделение краев + фильтр «максимума»

        public Form1()
        {
            InitializeComponent();
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                filename = openFileDialog1.FileName;
                im = new Bitmap(filename);
                btp = (Bitmap)im;

                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                button2.Enabled = true;
                pictureBox1.Image = im;
                pictureBox1.Update();
                x = 0;
                y = 0;
                width = pictureBox1.Image.Width;
                height = pictureBox1.Image.Height;
            }
        }

        private void median_filter()
        {
           
        //    Bitmap btp1 = (Bitmap)pictureBox1.Image.Clone();
        //    System.Drawing.Imaging.BitmapData btpd = btp.LockBits(new Rectangle(x, y, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Canonical);
            Color c,c1;
            for (int i = x; i < x + width; ++i)
                for (int j = y; j < y + height; ++j)
                {
                    int sumr = 0, sumg = 0, sumb = 0;
                    int cr = 0, cg = 0, cb = 0;
                    for (int k = i - medianRadius; k <= i + medianRadius; ++k)
                        for (int l = j - medianRadius; l <= j + medianRadius; ++l)
                        {
                            if (k > 0 && k < pictureBox1.Image.Width && l > 0 && l < pictureBox1.Image.Height)
                            {
                                c = input[k, l];
                                sumr += c.R;
                                sumg += c.G;
                                sumb += c.B;
                                ++cr; ++cg; ++cb;
                            }
                        }
                    c1 = Color.FromArgb(sumr / cr, sumg / cg, sumb / cb);
                    output[i, j]=c1;
                }
       //     pictureBox1.Image = btp1;
      //      btp.UnlockBits(btpd);
     //       btp = (Bitmap) btp1.Clone();
     //       pictureBox1.Update();
            
    //        if (radioButton3.Checked)
    //            orig = (Bitmap)pictureBox1.Image.Clone();
        }

        private void sobel()
        {
       //     if (radioButton3.Checked)
       //         pictureBox1.Image = (Bitmap)orig.Clone(); 
       //     Bitmap btp1 = (Bitmap)pictureBox1.Image.Clone();
       //     System.Drawing.Imaging.BitmapData btpd = btp.LockBits(new Rectangle(x, y, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Canonical);
            for (int i = x; i < x + width; ++i)
                for (int j = y; j < y + height; ++j)
                {
                    if (i > 0 && i < pictureBox1.Image.Width - 1 && j > 0 && j < pictureBox1.Image.Height - 1)
                    {
                        myColor res = calcSobelForPoint(i, j);
                        output[i, j] = Color.FromArgb(Math.Min(255, res.R), Math.Min(255, res.G), Math.Min(255, res.B));
                    }
                    else
                        output[i, j] = Color.Black;
                }
       //     pictureBox1.Image = btp1;
       //     btp.UnlockBits(btpd);
      //      btp = (Bitmap)btp1.Clone();
      //      pictureBox1.Update();
      //      if (radioButton3.Checked)
     //           orig = (Bitmap)pictureBox1.Image.Clone();
        }

        private myColor calcSobelForPoint(int x, int y)
        {            
            Color z1 = input[x-1,y-1];
            Color z2 = input[x,y-1];
            Color z3 = input[x+1,y-1];
            Color z4 = input[x-1,y];
            Color z6 = input[x+1,y];
            Color z7 = input[x-1,y+1];
            Color z8 = input[x,y+1];
            Color z9 = input[x+1,y+1];
            myColor k1, k2, k3, k4;
            k1 = sumColors(z7, z8, z9);
            k2 = sumColors(z1, z2, z3);
            k1 = deductColors(k1, k2);
            k3 = sumColors(z3, z6, z9);
            k4 = sumColors(z1, z4, z7);
            k3 = deductColors(k3, k4);
            myColor res;
            res.R = k1.R + k3.R;
            res.G = k1.G + k3.G;
            res.B = k1.B + k3.B;
            return res;

        }

        private myColor sumColors(Color c1, Color c2, Color c3)
        {
            int sumr = 0, sumg = 0, sumb = 0;
            sumr = c1.R + 2 * c2.R + c3.R;
            sumg = c1.G + 2 * c2.G + c3.G;
            sumb = c1.B + 2 * c2.B + c3.B;
            myColor res;
            res.R = sumr;
            res.G = sumb;
            res.B = sumb;
            return res;
        }

        private myColor deductColors(myColor c1, myColor c2)
        {
            int sumr = 0, sumg = 0, sumb = 0;
            sumr = Math.Abs(c1.R - c2.R);
            sumg = Math.Abs(c1.G - c2.G);
            sumb = Math.Abs(c1.B - c2.B);
            myColor res;
            res.R = sumr;
            res.G = sumb;
            res.B = sumb;
            return res;
        }

        private void MaxFilter()
        {
      //      if (radioButton3.Checked)
     //           pictureBox1.Image = (Bitmap)orig.Clone();           
     //       Bitmap btp1 = (Bitmap)pictureBox1.Image.Clone();
            Color c, maxColor = Color.Black;
    //        Color[,] matr = toColorMatr(btp);
    //        Color[,] resmatr = (Color[,])matr.Clone();
            for (int i = x; i < x + width; ++i)
                for (int j = y; j < y + height; ++j)
                {
                    int maxComp = 0;
                    for (int k = i - maxFilterRadius; k <= i + maxFilterRadius; ++k)
                        for (int l = j - medianRadius; l <= j + maxFilterRadius; ++l)
                        {
                            int sumComp = 0;
                            if (k > 0 && k < pictureBox1.Image.Width && l > 0 && l < pictureBox1.Image.Height)
                            {
                                c = input[k,l];
                                sumComp += c.R;
                                sumComp += c.G;
                                sumComp += c.B;
                                if (sumComp > maxComp)
                                {
                                    maxComp = sumComp;
                                    maxColor = c;
                                }
                            }
                        }
                    if (maxComp > 0)
                        output[i,j] = maxColor;
                    else
                        output[i,j] = input[i,j];
                }
    //        btp1 = toBitmap(resmatr, width, height);
    //        pictureBox1.Image = btp1;
   //         btp = (Bitmap)btp1.Clone();
            
        }

        private Color[,] toColorMatr(Bitmap btp_, System.Drawing.Imaging.PixelFormat pf = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
  //          System.Drawing.Imaging.BitmapData btpd = btp_.LockBits(new Rectangle(x, y, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, pf);
            Color[,] result = new Color[btp_.Width, btp_.Height];
//            unsafe
 //           {
  //              byte* p = (byte*)(void*)btpd.Scan0.ToPointer();
 //               int stopAddress = (int)p + btpd.Width * btpd.Height;
                for (int i = 0; i < btp_.Width; ++i)
                    for (int j = 0; j <  btp_.Height; ++j)
                        result[i, j] = btp_.GetPixel(i, j);
//            }
 //           btp_.UnlockBits(btpd);
            return result;
        }
        
        private Bitmap toBitmap(Color[,] matr, int width, int height)
        {
            Bitmap res = new Bitmap(width, height);
     //       System.Drawing.Imaging.BitmapData btpd = res.LockBits(new Rectangle(x, y, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
          //      byte* p = (byte*)(void*)btpd.Scan0.ToPointer();
        //        int stopAddress = (int)p + btpd.Stride * btpd.Height;
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                        res.SetPixel(i, j, Color.FromArgb((byte)matr[i, j].R, (byte)matr[i, j].G, (byte)matr[i, j].B));
     //       res.UnlockBits(btpd);
     //       Color[,] test = toColorMatr(res,System.Drawing.Imaging.PixelFormat.Format32bppArgb);            
            return res;
        }

        private Color[,] toAreaColorMatr(Bitmap btp_, System.Drawing.Imaging.PixelFormat pf = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
            //          System.Drawing.Imaging.BitmapData btpd = btp_.LockBits(new Rectangle(x, y, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, pf);
            Color[,] result = new Color[btp_.Width, btp_.Height];
            //            unsafe
            //           {
            //              byte* p = (byte*)(void*)btpd.Scan0.ToPointer();
            //               int stopAddress = (int)p + btpd.Width * btpd.Height;
            for (int i = x; i < btp_.Width; ++i)
                for (int j = y; j < btp_.Height; ++j)
                    result[i-x, j-y] = btp_.GetPixel(i, j);
            //            }
            //           btp_.UnlockBits(btpd);
            return result;
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X > pictureBox1.Width || e.Y < 0 || e.Y > pictureBox1.Height)
                return;
            if (radioButton4.Checked)
                return;
            mousedown = true;
            x = e.X;
            y = e.Y;
        }

        private void swap(ref int x, ref int x1)
        {
            int t = x;
            x = x1;
            x1 = t;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X > pictureBox1.Width || e.Y < 0 || e.Y > pictureBox1.Height)
                return;
            if (radioButton4.Checked || !mousedown)
                return;
     //       pictureBox1.Dispose();
            pictureBox1.Image = (Bitmap)orig.Clone();            
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            x1 = e.X;
            y1 = e.Y;
    /*        if (x1 < x)
               swap(ref x,ref x1);
            if (y1 < y)
                swap(ref y,ref y1);*/
            g.DrawRectangle(new Pen(Color.Green), Math.Min(x,x1), Math.Min(y,y1), Math.Abs(x1 - x), Math.Abs(y1 - y));
            pictureBox1.Update();

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
     //       if (e.X < 0 || e.X > pictureBox1.Width || e.Y < 0 || e.Y > pictureBox1.Height)
     //           return;
            if (radioButton4.Checked)
                return;
            mousedown = false;
            button2.Enabled = true;
            x = Math.Min(x, x1);
            y = Math.Min(y, y1);
            width = Math.Abs(x1 - x);
            height = Math.Abs(y1 - y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                if (radioButton3.Checked)
                    pictureBox1.Image = (Bitmap)orig.Clone();
                input = toColorMatr(btp);
                output = (Color[,])input.Clone();
                median_filter();
                input = (Color[,])output.Clone();

                sobel();
                input = (Color[,])output.Clone();

                MaxFilter();
                btp = toBitmap(output, pictureBox1.Image.Width, pictureBox1.Image.Height);
                pictureBox1.Image = btp;
                pictureBox1.Update();
                if (radioButton3.Checked)
                    orig = (Bitmap)pictureBox1.Image.Clone();

            }
            else if (radioButton1.Checked)
            {
                pictureBox2.Visible = true;
                pictureBox2.Location = new Point(10, 10);
                Bitmap newIm = bicubic();
                pictureBox2.Width = newIm.Width;
                pictureBox2.Height = newIm.Height;
                pictureBox2.Image = newIm;
                pictureBox2.Update();
            }
            button3.Enabled = true;
        }

        private void radioButton4_MouseClick(object sender, MouseEventArgs e)
        {
            x = 0;
            y = 0;
            width = pictureBox1.Image.Width;
            height = pictureBox1.Image.Height;
            allowdraw = false;
            button2.Enabled = true;
            pictureBox1.Image = new Bitmap(filename);
            resetImage();
        }

        private void radioButton3_MouseClick(object sender, MouseEventArgs e)
        {
            orig = (Bitmap)pictureBox1.Image.Clone();
            allowdraw = true;

            button2.Enabled = false;
            resetImage();
        }

        private void resetImage()
        {
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;
            im = new Bitmap(filename);
            pictureBox1.Image = im;
            btp = (Bitmap)im.Clone();
            orig = (Bitmap)btp.Clone();
            pictureBox1.Update();
            pictureBox2.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            resetImage();
        }

        private myColor toMyColor(Color c)
        {
            myColor res;
            res.R = c.R;
            res.G = c.G;
            res.B = c.B;
            return res;
        }

        private myColor deColors(myColor c, myColor c1)
        {
            myColor res;
            res.R = c.R - c1.R;
            res.G = c.G - c1.G;
            res.B = c.B - c1.B;
            return res;
      //      return Color.FromArgb(c.R - c1.R, c.G - c1.G, c.B - c1.B);
      //      return res;
        }

        private myColor sColors(myColor c, myColor c1)
        {
            myColor res;
            res.R = c.R + c1.R;
            res.G = c.G + c1.G;
            res.B = c.B + c1.B;
            return res;
        }

        private myColor prodColors(myColor c, double d)
        {
            myColor res;
            res.R = (int)(c.R*d);
            res.G = (int)(c.G*d);
            res.B = (int)(c.B*d);
            return res;
        }

        private myColor getCubi(myColor a0, myColor a1, myColor a2, myColor a3, double dx)
        {
            myColor res;
            res = sColors(a0, prodColors(a1, dx));
            res = sColors(res, prodColors(a2, dx * dx));
            res = sColors(res, prodColors(a3, dx * dx * dx));
            return res;
        }

        private Color toColor(myColor c)
        {
            myColor tmp = c;
            if (tmp.R < 0)
                tmp.R = 0;
            if (tmp.R > 255)
                tmp.R = 255;
            if (tmp.G < 0)
                tmp.G = 0;
            if (tmp.G > 255)
                tmp.G = 255;
            if (tmp.B < 0)
                tmp.B = 0;
            if (tmp.B > 255)
                tmp.B = 255;
            return Color.FromArgb(tmp.R,tmp.G,tmp.B);
        }

        private int getFloat(double d)
        {
            double t = d - Math.Ceiling(d);
            if (t >= 0.5)
                return 1;
            else
                return 0;
        }

        private Bitmap bicubic()
        {
            int newx = x;
            int newy = y;
            int newWidth = (int)zoomk * width;
            int newHeight = (int)zoomk * height;
            
            Color[,] newData = new Color[newWidth,newHeight];
            Color[,] data = new Color[width,height];
            if (radioButton3.Checked)
                data = toAreaColorMatr(btp);
            else
                data = toColorMatr(btp);

            double tx = (double) 1.0/zoomk; // How many zoom per X and Y
            double ty = (double) 1.0/zoomk;

            double dx, dy;
                   
            
            myColor d0,d2,d3,a0,a1,a2,a3;
            myColor[] C = new myColor[4];
            myColor resColor;
            for(int i=2; i<newWidth-2/tx; i++)
            for(int j=2; j<newHeight-2/tx; j++)
            {

               x = (int)(tx*i); // Getting pixels from original image
               y = (int)(ty * j);

               dx = tx * i - x;  // Get the difference from int points of original
               dy = ty * j - y ;           
          

                  for(int l=0;l<=3;l++)
                  {

                      d0 = deColors(toMyColor(data[x - 1, y - 1 + l]), toMyColor(data[x, y - 1 + l])); //Finding derivatives X
                      d2 = deColors(toMyColor(data[x + 1, y - 1 + l]), toMyColor(data[x, y - 1 + l]));
                      d3 = deColors(toMyColor(data[x + 2, y - 1 + l]), toMyColor(data[x, y - 1 + l]));
                      a0 = toMyColor(data[x, y - 1 + l]);                                               // Finding coeefs
                     a1 = sColors(sColors(d2,prodColors(d0,(-1.0/3.0))),prodColors(d3,(-1.0/6.0)));
                     a2 = sColors(prodColors(d0,(1.0/2.0)),prodColors(d2,(1.0/2.0)));
                     a3 = sColors(sColors(prodColors(d2,(1.0/2.0)),prodColors(d0,(-1.0/6.0))),prodColors(d3,(1.0/6.0)));
                     
                     C[l] = getCubi(a0,a1,a2,a3,dx); // Finding cub equation

                  }
                d0 = deColors(C[0],C[1]);   // Finding derivatives Y
                d2 = deColors(C[2],C[1]);
                d3 = deColors(C[3],C[1]);
                a0 = C[1];

                a1 = sColors(sColors(d2,prodColors(d0,(-1.0/3.0))),prodColors(d3,(-1.0/6.0)));          // Finding coeefs
                a2 = sColors(prodColors(d0,(1.0/2.0)),prodColors(d2,(1.0/2.0)));
                a3 = sColors(sColors(prodColors(d2,(1.0/2.0)),prodColors(d0,(-1.0/6.0))),prodColors(d3,(1.0/6.0)));

                resColor = getCubi(a0,a1,a2,a3,dy);         // Getting produced color from cub equation
                newData[i,j] = toColor(resColor);
                                
            }
            Bitmap newImage = toBitmap(newData, newWidth, newHeight);
            return newImage;   
        }

    }
}
