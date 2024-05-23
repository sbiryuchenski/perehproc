using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace perehproc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.Padding = new Padding(Left, Top, Right, Bottom);
            chart1.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.Crossing = 0;
            chart1.ChartAreas[0].AxisX.Crossing = 0;
            chart1.ChartAreas[0].AxisX.LineWidth = 1;
            chart1.Series.Clear();
            isUsedFlag = false;
        }
        GraphCounter graphCounter;
        bool isUsedFlag;

        //private void chart1_Click(object sender, EventArgs e)
        //{

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            bool noErrors = true;
            List<Point> pointList = new List<Point>();
            graphCounter = new GraphCounter();

            float k1=0, k2=0, k3=0, k4 = 0;
            try
            {
                k1 = (float)Convert.ToDouble(textBox1.Text);
                k2 = (float)Convert.ToDouble(textBox2.Text);
                k3 = (float)Convert.ToDouble(textBox3.Text);
                k4 = (float)Convert.ToDouble(textBox4.Text);
            }
            catch 
            {
                MessageBox.Show("Не верно введены данные");
                noErrors = false;
            };

            if(noErrors)
            {
                chart1.Series.Add(new Series()); // Добавляем ещё новый график по нажатию кнопки
                int gNumber = chart1.Series.Count - 1;
                chart1.Series[gNumber].ChartType = SeriesChartType.Spline;
                chart1.Series[gNumber].BorderWidth = 3;
                chart1.Series[gNumber].Color = System.Drawing.Color.Black;

                pointList = graphCounter.mainmethod(k1, k2, k3, k4);

                foreach (var point in pointList)
                {
                    chart1.Series[gNumber].Points.Add(point.X, point.Y);
                }

                try
                {
                    chart1.Series[gNumber].Name = $"x1 = {textBox1.Text}, x2 = {textBox2.Text}, x3 = {textBox3.Text}, x4 = {textBox4.Text}";
                }
                catch
                {
                    MessageBox.Show("График с такими данными уже существует");
                }
            }
            
        }

        private void clrBtn_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            isUsedFlag = false;
        }
    }

}
