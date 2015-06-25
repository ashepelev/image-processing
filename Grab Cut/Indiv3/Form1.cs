using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Indiv3
{
    public partial class Form1 : Form
    {
        string filename;
        Image im;
        Bitmap btp,orig;
        int x = 0, y = 0, x1, y1;
        int width, height;
        bool mousedown = false;
        bool imloaded = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                filename = openFileDialog1.FileName;
                im = new Bitmap(filename);
                btp = (Bitmap)im;

                pictureBox1.Image = im;
                pictureBox1.Update();
                imloaded = true;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X > pictureBox1.Width || e.Y < 0 || e.Y > pictureBox1.Height || !imloaded)
                return;
            mousedown = true;
            x = e.X;
            y = e.Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mousedown)
                return;
            mousedown = false;
            x = Math.Min(x, x1);
            y = Math.Min(y, y1);
            width = Math.Abs(x1 - x);
            height = Math.Abs(y1 - y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X > pictureBox1.Width || e.Y < 0 || e.Y > pictureBox1.Height || !imloaded || !mousedown)
                return;
            pictureBox1.Image = (Bitmap)orig.Clone();
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            x1 = e.X;
            y1 = e.Y;
            g.DrawRectangle(new Pen(Color.Green), Math.Min(x, x1), Math.Min(y, y1), Math.Abs(x1 - x), Math.Abs(y1 - y));
            pictureBox1.Update();

        }



    }
}
