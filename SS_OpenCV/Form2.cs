using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace SS_OpenCV
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(int[] gray, int[,]RGB)
        {
            InitializeComponent();

            PointPairList grayPoints = new PointPairList();
            for(int i=0;i<256;i++){
                grayPoints.Add(i,gray[i]);
            }
            zedGraph.GraphPane.AddCurve("Grey", grayPoints, Color.Gray);

            PointPairList redPoints = new PointPairList();
            for (int i = 0; i < 256; i++) {
                grayPoints.Add(i, RGB[0,i]);
            }
            zedGraph.GraphPane.AddCurve("Red", redPoints, Color.Red);

            PointPairList greenPoints = new PointPairList();
            for (int i = 0; i < 256; i++){
                greenPoints.Add(i, RGB[1,i]);
            }
            zedGraph.GraphPane.AddCurve("Green", greenPoints, Color.Green);

            PointPairList bluePoints = new PointPairList();
            for (int i = 0; i < 256; i++){
                bluePoints.Add(i, RGB[2,i]);
            }
            zedGraph.GraphPane.AddCurve("Blue", bluePoints, Color.Blue);

            //zedGraph.axis.Scale.Min = 0;
            //axis.Scale.Max = 100;

            //zedGraph.IsShowPointValues = true;
            //zedGraph.IsSynchronizeYAxes = true;
            zedGraph.GraphPane.XAxis.Scale.Min = 0;
            zedGraph.GraphPane.XAxis.Scale.Max = 255;

            zedGraph.GraphPane.YAxis.Scale.Min = 0;
            zedGraph.GraphPane.YAxis.Scale.Max = 2000;

            //DataPointCollection list1 = zed.Series[0].Points;
            //for (int i = 0; i < array.Length; i++)
            //{
            //    list1.AddXY(i, array[i]);
            //}
            //chart1.Series[0].Color = Color.Gray;
            //chart1.ChartAreas[0].AxisX.Maximum = 255;
            //chart1.ChartAreas[0].AxisX.Minimum = 0;
            //chart1.ChartAreas[0].AxisX.Title = "Intensity";
            //chart1.ChartAreas[0].AxisY.Title = "Number of pixels";
            //chart1.ResumeLayout();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
