using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AQUATOX.AQTSegment;
using Globals;
using Data;
using System.Linq;


namespace GUI.AQUATOX
{
    public partial class OutputForm : Form
    {
        private Chart chart1 = new Chart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        Series series1 = new Series();

        public AQTSim aQTS = null;
        public AQUATOXSegment outSeg = null;

        public OutputForm()
        {
            InitializeComponent();

            // 
            // chart1
            // 
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            SuspendLayout();

            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);

            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(30, 70);
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(720, 410);
            chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            chart1.Series.Clear();
            this.chart1.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart1_CustomizeLegend_1);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);

            Controls.Add(chart1);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void ShowOutput(AQTSim aQ)
        {
            aQTS = aQ;
            OutputBox.Items.Clear();
            if (aQTS.SavedRuns == null) { System.Windows.Forms.MessageBox.Show("No Runs are Saved."); return; }

            foreach (KeyValuePair<string, AQUATOXSegment> entry in aQTS.SavedRuns)
            {
                OutputBox.Items.Add(entry.Key);
            }

            OutputBox.SelectedIndex = OutputBox.Items.Count - 1;
            Application.DoEvents();
            OutputBox.Visible = true;

            Show();
        
        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestResult resultExplode = chart1.HitTest(e.X, e.Y);
            if (resultExplode.Series != null)

            {
                if (resultExplode.Object is LegendItem)
                {
                    LegendItem legendItem = (LegendItem)resultExplode.Object;
                    Series selectedSeries = (Series)legendItem.Tag;
                    if (selectedSeries != null)
                        selectedSeries.Enabled = !selectedSeries.Enabled;
                    chart1.ChartAreas[0].RecalculateAxesScale();
                }

                string msgstr = resultExplode.Series.Name;
                if (resultExplode.PointIndex > 0)
                {
                    msgstr = msgstr + ": " +
                    resultExplode.Series.Points[resultExplode.PointIndex].YValues[0] + " \n " +
                    System.DateTime.FromOADate(resultExplode.Series.Points[resultExplode.PointIndex].XValue);
                    System.Windows.Forms.MessageBox.Show(msgstr);
                }

            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.DoEvents();

            aQTS.SavedRuns.TryGetValue(OutputBox.Text, out outSeg);

            DisplayGraph();
        }

        private void chart1_CustomizeLegend_1(object sender, CustomizeLegendEventArgs e)
        {

            e.LegendItems.Clear();
            foreach (var series in this.chart1.Series)
            {
                var legendItem = new LegendItem();
                legendItem.SeriesName = series.Name;
                legendItem.ImageStyle = LegendImageStyle.Rectangle;
                legendItem.BorderColor = Color.Transparent;
                legendItem.Name = series.Name + "_legend_item";

                int i = legendItem.Cells.Add(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleCenter);
                legendItem.Cells.Add(LegendCellType.Text, series.Name, ContentAlignment.MiddleCenter);

                if (series.Enabled)
                { legendItem.Color = series.Color; legendItem.BorderColor = Color.Black; }
                else
                { legendItem.Color = Color.FromArgb(100, series.Color); legendItem.BorderColor = Color.White; }

                legendItem.Tag = series;
                e.LegendItems.Add(legendItem);
            }

        }



        public void DisplayGraph()

        {
            TStateVariable TSV1 = null;

            chart1.Series.Clear();
            int sercnt = 0;
            string outtxt = "";

            foreach (TStateVariable TSV in outSeg.SV) if (TSV.SVoutput != null)
                {
                    TSV1 = TSV; // identify TSV1 with an output that is not null
                    int cnt = 0;
                    if (sercnt == 0) outtxt = "Date, ";

                    List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                    for (int col = 1; col <= vallist.Count(); col++)
                    {
                        string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
                             TSV.SVoutput.Metadata["Name_" + col.ToString()] +
                             " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";
                        Series ser = chart1.Series.Add(sertxt);

                        if (col == 1) outtxt = outtxt + sertxt.Replace(",", "") + ", ";  // suppress commas in name for CSV output

                        ser.ChartType = SeriesChartType.Line;
                        ser.BorderWidth = 2;
                        ser.MarkerStyle = MarkerStyle.Diamond;
                        ser.Enabled = false;
                        sercnt++;

                        for (int i = 0; i < TSV.SVoutput.Data.Keys.Count; i++)
                        {
                            ITimeSeriesOutput ito = TSV.SVoutput;
                            string datestr = ito.Data.Keys.ElementAt(i).ToString();
                            Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[col - 1]);
                            ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                            cnt++;
                        }

                    }

                }
        }
    }
}
