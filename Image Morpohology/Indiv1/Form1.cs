using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Indiv1
{
    public partial class Form1 : Form
    {
        string filename;
        Image im;
        Bitmap btp;

        byte[,] matr;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                filename = openFileDialog1.FileName;
                im = new Bitmap(filename);
                pictureBox1.Image = im;
                button2.Enabled = true;
                matr = new byte[im.Width, im.Height];
                loadMatrix();
            }
        }

        private void loadMatrix()
        {
            btp = (Bitmap) im;
            Color pixelColor;
            for (int i = 0; i < im.Width; ++i)
                for (int j = 0; j < im.Height; ++j)
                {
                    pixelColor = btp.GetPixel(i, j);
                    if (pixelColor.A > 0 && pixelColor.R > 127 && pixelColor.G > 127 && pixelColor.B > 127)
                        matr[i, j] = 1;
                    else
                        matr[i, j] = 0;
                }
        }

        private void DrawMatrix(BinImage bi)
        {
      //      pictureBox1.Dispose();
            btp = new Bitmap(bi.Width, bi.Height);
            for (int i = 0; i <  bi.Width; ++i)
                for (int j = 0; j < bi.Height; ++j)
                    if (bi.matr[i, j] == 1)
                        btp.SetPixel(i, j, Color.White);
                    else
                        btp.SetPixel(i, j, Color.Black);
            pictureBox1.Image = btp;
            pictureBox1.Invalidate();
            pictureBox1.Update();
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

        private void button2_Click(object sender, EventArgs e)
        {
            
            
            BinImage bi = new BinImage(matr, im.Width, im.Height);
            bi.LoadMask(createCircleMatr(10));
            bi.Dilation();            
            DrawMatrix(bi);
            MessageBox.Show("Dilation Done");
            bi.Erosion();
            DrawMatrix(bi);
        }
    }
}
