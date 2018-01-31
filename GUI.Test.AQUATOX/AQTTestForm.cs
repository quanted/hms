using System;
using System.IO;
using System.Windows.Forms;
using AQUATOX.AQTSegment;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using System.Linq;

namespace AQUATOX.GUI.Test
{

    public partial class AQTTestForm : Form
    {
        private TextBox textBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Button loadJSON;
        private Button saveJSON;
        private Button button4;
        private Button button1;
        public AQTSim aQTS = null; 
//        public JsonSerializerSettings loJsonSerializerSettings;

        public AQTTestForm()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
        }

        
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.loadJSON = new System.Windows.Forms.Button();
            this.saveJSON = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Simple Integration Test";
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
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(31, 225);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(649, 288);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);
            // 
            // loadJSON
            // 
            this.loadJSON.Location = new System.Drawing.Point(342, 12);
            this.loadJSON.Name = "loadJSON";
            this.loadJSON.Size = new System.Drawing.Size(94, 23);
            this.loadJSON.TabIndex = 4;
            this.loadJSON.Text = "Load JSON";
            this.loadJSON.UseVisualStyleBackColor = true;
            this.loadJSON.Click += new System.EventHandler(this.button2_Click);
            // 
            // saveJSON
            // 
            this.saveJSON.Location = new System.Drawing.Point(453, 12);
            this.saveJSON.Name = "saveJSON";
            this.saveJSON.Size = new System.Drawing.Size(94, 23);
            this.saveJSON.TabIndex = 5;
            this.saveJSON.Text = "Save JSON";
            this.saveJSON.UseVisualStyleBackColor = true;
            this.saveJSON.Click += new System.EventHandler(this.saveJSON_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(563, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(94, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Integrate";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // AQTTestForm
            // 
            this.ClientSize = new System.Drawing.Size(710, 533);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.saveJSON);
            this.Controls.Add(this.loadJSON);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "AQTTestForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        public void DisplaySVs()

        {
            string outtxt = "";

            //outtxt = outtxt + "Times: ";
            //foreach (DateTime Times in aQTS.AQTSeg.SV.restimes)
            //    outtxt = outtxt + Times + ", ";
            //outtxt = outtxt + Environment.NewLine;

            chart1.Series.Clear();

            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
            {
                int cnt = 0;
                System.Windows.Forms.DataVisualization.Charting.Series ser = chart1.Series.Add(TSV.PName);
                ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                ser.BorderWidth = 2;
                ser.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;

                for (int i = 0; i < TSV.output.Data.Keys.Count; i++)
                {
                    ITimeSeriesOutput ito = TSV.output;
                    string datestr = ito.Data.Keys.ElementAt(i).ToString();
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                    outtxt = outtxt + datestr + ", "+ Val.ToString()+"; ";
                    ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                    cnt++;
                }
                outtxt = outtxt + Environment.NewLine;
                outtxt = outtxt + Environment.NewLine;


                //foreach (double Vals in TSV.Results)
                //{
                //    outtxt = outtxt + Vals + ", ";
                //    ser.Points.AddXY(aQTS.AQTSeg.SV.restimes[cnt], Vals);
                //    cnt++;
                //}
                //outtxt = outtxt + Environment.NewLine;
                //outtxt = outtxt + Environment.NewLine;
            }

            textBox1.Text = outtxt;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            aQTS = new AQTSim();
            aQTS.AQTSeg = new AQUATOXSegment();
            aQTS.AQTSeg.RunTest();
            DisplaySVs();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)  //loadbutton

        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Test File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                // FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                string json = File.ReadAllText(openFileDialog1.FileName);
                AQTSim Sim = new AQTSim();
                string err = Sim.Instantiate(json);
                aQTS = Sim;

                if (err == "") textBox1.Text = "Read File " + openFileDialog1.FileName;
                else textBox1.Text = err;
             }
        }


            private void saveJSON_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Test File|*.txt";
            saveFileDialog1.Title = "Save to JSON File";
            saveFileDialog1.ShowDialog();


            if (saveFileDialog1.FileName != "")
            {
                string jsondata="";
                string errmessage = aQTS.SaveJSON(ref jsondata);
                if (errmessage == "") File.WriteAllText(saveFileDialog1.FileName, jsondata);
                else textBox1.Text = errmessage;
            }
        }

        private void button4_Click(object sender, EventArgs e) // integrate
        {
            if (aQTS == null) textBox1.Text = "Simulation not Instantiated";
            else {
                string errmessage = aQTS.Integrate();
                if (errmessage == "") DisplaySVs();
                else textBox1.Text = errmessage;
            }
        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestResult resultExplode = chart1.HitTest(e.X, e.Y);
            if (resultExplode.Series!=null)

            {
                string msgstr = resultExplode.Series.Name;
                if (resultExplode.PointIndex > 0)
                    {
                    msgstr = msgstr + ": " +
                    resultExplode.Series.Points[resultExplode.PointIndex].YValues[0] + " \n " +
                    DateTime.FromOADate(resultExplode.Series.Points[resultExplode.PointIndex].XValue);
                    }
                System.Windows.Forms.MessageBox.Show(msgstr);
            }

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
