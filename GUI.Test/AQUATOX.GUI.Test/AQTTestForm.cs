using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AQUATOX.AQTSegment;

namespace AQUATOX.GUI.Test
{
    public partial class AQTTestForm : Form
    {
        private TextBox textBox1;
        private Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Button button1;

        public AQTTestForm()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(31, 51);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(649, 157);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(155, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "API Calls";
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(31, 225);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(649, 288);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            // 
            // AQTTestForm
            // 
            this.ClientSize = new System.Drawing.Size(710, 533);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "AQTTestForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        private void Button1_Click(object sender, EventArgs e)
        {
            string outtxt = "";
            AQUATOXSegment aQT = new AQUATOXSegment();
            aQT.RunTest();

            outtxt = outtxt + "Times: ";
            foreach (DateTime Times in aQT.SV.restimes)
                outtxt = outtxt + Times + ", ";
            outtxt = outtxt + Environment.NewLine;

            foreach (TStateVariable TSV in aQT.SV)
            {
                int cnt = 0;
                System.Windows.Forms.DataVisualization.Charting.Series ser = chart1.Series.Add(TSV.PName);
                ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                ser.BorderWidth = 2;
                ser.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
                foreach (double Vals in TSV.Results)
                {
                    outtxt = outtxt + Vals + ", ";
                    ser.Points.AddXY(aQT.SV.restimes[cnt], Vals);
                    cnt++;
                }
                outtxt = outtxt + Environment.NewLine;
                outtxt = outtxt + Environment.NewLine;
            }

            textBox1.Text = outtxt;

        }


    }
}
