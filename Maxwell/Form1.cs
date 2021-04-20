using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxwell
{
    public partial class Form1 : Form
    {

        public static Form1 ff1;
        public static Form2 ff2;

        int N;
        double R, Vmax, Xmax, Ymax, dt, K;
        double[] X, Y, Vx, Vy;
        Random rndm = new Random();

        double p_x, p_y, p2, V_par1_x, V_par1_y, V_perp1_x, V_perp1_y, V_par2_x, V_par2_y, V_perp2_x, V_perp2_y;

        private void посмотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ff2 = new Form2();
            ff2.Visible = true;
            посмотретьToolStripMenuItem.Enabled = false;
        }

        int t; 
        double[] CS;
        int[] CN;

        Bitmap bmp1, bmp2;
        Graphics gr1, gr2;
        Pen P1 = new Pen(Color.Red, 2);
        Pen P2 = new Pen(Color.Green, 2);
        SolidBrush Br = new SolidBrush(Color.Yellow);

        private void Form1_Load(object sender, EventArgs e)
        {
            ff1 = this;
            bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr1 = Graphics.FromImage(bmp1);
            bmp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr2 = Graphics.FromImage(bmp2);
        }

        public Form1()
        {
            InitializeComponent();
        }
    
        private void start_button_Click(object sender, EventArgs e)
        {
            t = 0;
            N = int.Parse(textBox1.Text);
            R = double.Parse(textBox2.Text);
            Vmax = double.Parse(textBox3.Text);
            Xmax = double.Parse(textBox4.Text) / 2;
            Ymax = double.Parse(textBox5.Text) / 2;
            dt = double.Parse(textBox6.Text);
            K = double.Parse(textBox7.Text);

            X = new double[N];
            Y = new double[N];
            Vx = new double[N];
            Vy = new double[N];
            CS = new double[N];
            CN = new int[N];

            for (int i = 0; i < N; i++)
            {
                CS[i] = 0;
                CN[i] = 0;

                X[i] = (2*rndm.NextDouble() - 1) * (Xmax - R);
                Y[i] = (2*rndm.NextDouble() - 1) * (Ymax - R);
                Vx[i] = (2 * rndm.NextDouble() - 1) * Vmax;
                Vy[i] = (2 * rndm.NextDouble() - 1) * Vmax;
            }


            gr1.Clear(pictureBox1.BackColor);
            gr2.Clear(pictureBox1.BackColor);
            // Xscr = (int) (bmp1.width /2 + K * X)
            // Yscr = (int) (bmp1.width /2 - K * Y)

            //Двумерный случай
            gr1.DrawRectangle(P2, (int)(bmp1.Width/2 - K*Xmax), (int)(bmp1.Height / 2 - K * Ymax), (int)(2*K*Xmax), (int)(2*K*Ymax));
            gr2.DrawImage(bmp1, 0, 0);

            for(int i = 0; i < N; i++)
            {
                gr2.DrawEllipse(P1, (int)(bmp1.Width / 2 + K * (X[i] - R)), (int)(bmp1.Height / 2 - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
                gr2.FillEllipse(Br, (int)(bmp1.Width / 2 + K * (X[i] - R)), (int)(bmp1.Height / 2 - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
            }
            pictureBox1.Image = bmp2;

            start_button.Enabled = false;
            stop_button.Enabled = true;
            посмотретьToolStripMenuItem.Enabled = true;
            timer.Enabled = true;
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            start_button.Enabled = true;
            stop_button.Enabled = false;
            timer.Enabled = false;
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            t++;
            textBox8.Text = (t * dt).ToString();
            //Движение частиц
            for (int i = 0; i < N; i++)
            {
                X[i] += Vx[i] * dt;
                Y[i] += Vy[i] * dt;
                CS[i] += Math.Sqrt(Vx[i] * Vx[i] + Vy[i] * Vy[i])*dt;
                // Проверяем соударение со стенками
                if ((X[i] >= Xmax - R) && (Vx[i] > 0)) Vx[i] = -Vx[i];
                if ((X[i] <= -Xmax + R) && (Vx[i] < 0)) Vx[i] = -Vx[i];
                if ((Y[i] >= Ymax - R) && (Vy[i] > 0)) Vy[i] = -Vy[i];
                if ((Y[i] <= -Ymax + R) && (Vy[i] < 0)) Vy[i] = -Vy[i];
            }


            //Соударение частиц между собой
            //i - 1, j - 2
            for (int i = 0; i<N-1; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    p_x = X[j] - X[i];
                    p_y = Y[j] - Y[i];
                    p2 = p_x * p_x + p_y * p_y;
                    if (p2 <= 4 * R * R)
                    {
                        V_par1_x = (Vx[i] * p_x + Vy[i] * p_y) * p_x / p2;
                        V_par1_y = (Vx[i] * p_x + Vy[i] * p_y) * p_y / p2;
                        V_par2_x = (Vx[j] * p_x + Vy[j] * p_y) * p_x / p2;
                        V_par2_y = (Vx[j] * p_x + Vy[j] * p_y) * p_y / p2;

                        if ((V_par2_x - V_par1_x)*p_x + (V_par2_y - V_par1_y) * p_y < 0) 
                        {
                            V_perp1_x = Vx[i] - V_par1_x;
                            V_perp1_y = Vy[i] - V_par1_y;
                            V_perp2_x = Vx[j] - V_par2_x;
                            V_perp2_y = Vy[j] - V_par2_y;


                            Vx[i] = V_par2_x + V_perp1_x;
                            Vy[i] = V_par2_y + V_perp1_y;
                            Vx[j] = V_par1_x + V_perp2_x;
                            Vy[j] = V_par1_y + V_perp2_y;
                            CN[i]++;
                            CN[j]++;
                        }
                    }
                }
            }


            gr2.DrawImage(bmp1, 0, 0);
            for (int i = 0; i < N; i++)
            {
                gr2.DrawEllipse(P1, (int)(bmp1.Width / 2 + K * (X[i] - R)), (int)(bmp1.Height / 2 - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
                gr2.FillEllipse(Br, (int)(bmp1.Width / 2 + K * (X[i] - R)), (int)(bmp1.Height / 2 - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
            }
            pictureBox1.Image = bmp2;
        }
    }
}