using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class Form1 : Form
    {
        public List<System.Windows.Forms.TextBox> coefficientsList = new List<TextBox>();
        public float[,] matrix = new float[3,3];
        public float weight = 0;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            lbFiltry.Items.Add("Mean 3x3");
            lbFiltry.Items.Add("Sharpen 3x3");
            lbFiltry.Items.Add("Gaussian 3x3");
            lbFiltry.Items.Add("Laplacian hard 3x3");
            lbFiltry.Items.Add("Vertical lines 3x3");
            try
            {
                coefficientsList.Add(tbCoef1);
                coefficientsList.Add(tbCoef2);
                coefficientsList.Add(tbCoef3);
                coefficientsList.Add(tbCoef4);
                coefficientsList.Add(tbCoef5);
                coefficientsList.Add(tbCoef6);
                coefficientsList.Add(tbCoef7);
                coefficientsList.Add(tbCoef8);
                coefficientsList.Add(tbCoef9);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (lbFiltry.SelectedIndex)
                {
                    case 0:
                        for (int i = 0; i < 9; i++)
                        {
                            coefficientsList[i].Text = "1";
                        }
                        tbWeight.Text = "9";
                        break;
                    case 1:
                        for (int i = 0; i < 9; i++)
                        {
                            if (i == 4)
                                coefficientsList[i].Text = "9";
                            else
                                coefficientsList[i].Text = "-1";
                        }
                        tbWeight.Text = "1";
                        break;
                    case 2:
                        for (int i = 0; i < 9; i++)
                        {
                            if (i == 4)
                                coefficientsList[i].Text = "4";
                            else if (i % 2 == 0)
                                coefficientsList[i].Text = "1";
                            else
                                coefficientsList[i].Text = "2";
                        }
                        tbWeight.Text = "16";
                        break;
                    case 3:
                        for (int i = 0; i < 9; i++)
                        {
                            if (i == 4)
                                coefficientsList[i].Text = "4";
                            else if (i % 2 == 0)
                                coefficientsList[i].Text = "1";
                            else
                                coefficientsList[i].Text = "-2";
                        }
                        tbWeight.Text = "1";
                        break;
                    case 4:
                        coefficientsList[0].Text = "0";
                        coefficientsList[1].Text = "0";
                        coefficientsList[2].Text = "0";
                        coefficientsList[3].Text = "-1";
                        coefficientsList[4].Text = "2";
                        coefficientsList[5].Text = "-1";
                        coefficientsList[6].Text = "0";
                        coefficientsList[7].Text = "0";
                        coefficientsList[8].Text = "0";
                        tbWeight.Text = "1";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);}
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fillMatrix()
        {
            try
            {
                for(int i = 0; i < 9; i++)
                {
                    matrix[((int)i/3),i%3] = (float)Convert.ToDouble(coefficientsList[i].Text);
                }
                weight = (float)Convert.ToDouble(tbWeight.Text);
            }
            catch (Exception exe) { MessageBox.Show(exe.Message); }

        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            fillMatrix();
            this.Hide();
        }
    }
}
