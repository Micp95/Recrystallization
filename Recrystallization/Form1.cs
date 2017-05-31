using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recrystallization
{
    public partial class Form1 : Form
    {
        Simulation simulation;
        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        
        private void Initialize()
        {
            selectLib.SelectedIndex = 0;
            selectBound.SelectedIndex = 0;
            selectRand.SelectedIndex = 0;

            simulation = new Simulation();
        }

        private void Render()
        {
            var map = simulation.GetMap();
            if (map != null)
                pictureBox1.Image = ResizeBitmap(map, pictureBox1.Width, pictureBox1.Height);
            //    pictureBox1.Image = new Bitmap(map, new Size(pictureBox1.Width, pictureBox1.Height));
        }
        private Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            }
            return result;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            Render();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateSeedsMethod generateSeedsMethod = (GenerateSeedsMethod)selectRand.SelectedIndex;
            BoundaryValue boundaryValue = (BoundaryValue)selectBound.SelectedIndex;
            int[] parameters = null;
            switch (generateSeedsMethod)
            {
                case GenerateSeedsMethod.Evenly:
                    parameters = new int[2] { int.Parse(textBoxX.Text), int.Parse(textBoxY.Text) };
                    break;
                case GenerateSeedsMethod.RandomR:
                    parameters = new int[1] { int.Parse(textBoxR.Text) };
                    break;
                case GenerateSeedsMethod.ClickMethod:
                case GenerateSeedsMethod.Random:
                default:
                    break;
            }

            simulation.GenerateSeeds(int.Parse(textBoxWidth.Text), int.Parse(textBoxHeight.Text), generateSeedsMethod, boundaryValue,int.Parse(textBoxAmount.Text), parameters);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            NeighborhoodMethod neighborhoodMethod = (NeighborhoodMethod)selectLib.SelectedIndex;
            simulation.StartSimulation(neighborhoodMethod,int.Parse(textBoxK.Text));
            timer1.Enabled = true;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int k = 1;
            bool flag = false;
            while(k > 0)
            {
                flag =simulation.NextStep();
                k--;
            }
            Render();

            if (!flag)
            {
                timer1.Enabled = false;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs arg = (MouseEventArgs)e;

            int w = int.Parse(textBoxWidth.Text);
            int h = int.Parse(textBoxHeight.Text);


            int wm = pictureBox1.Width;
            int hm = pictureBox1.Height;


            int pX = (int)(((double)w / (double)wm) * arg.X);
            int pY = (int)(((double)h / (double)hm) * arg.Y);
            simulation.AddSeed(pX, pY);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            simulation.StartMC(int.Parse(textBoxAmount.Text), int.Parse(textBoxWidth.Text), int.Parse(textBoxHeight.Text));
        }
    }
}
