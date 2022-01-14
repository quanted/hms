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
using System.IO;

namespace GUI.AQUATOX
{
    public partial class OutputForm : Form
    {

        public class OChart : Chart
        {
            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            {
                try
                {
                    base.OnPaint(e);
                }
                catch (Exception ex)
                {
                    ChartAreas[0].AxisY.IsLogarithmic = false;
                    System.Windows.Forms.MessageBox.Show("Zero or negative values cannot be displayed on a logarithmic scale");
                    base.OnPaint(e);
                }
            }
        }

        string HelpTopic = "OutputScreen";
        private OChart chart1 = new OChart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        Series series1 = new Series();

        public AQTSim aQTS = null;
        public AQUATOXSegment outSeg = null;

        public OutputForm()
        {

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

            Graphics graphics = this.CreateGraphics();
            double ScaleX = graphics.DpiX / 96;
            double ScaleY = graphics.DpiY / 96;

            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(Convert.ToInt32(30 * ScaleX), Convert.ToInt32(70 * ScaleY));
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(Convert.ToInt32(750 * ScaleX), Convert.ToInt32(330 * ScaleY));
            chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            chart1.Series.Clear();
            this.chart1.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart1_CustomizeLegend_1);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);

            Controls.Add(chart1);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            InitializeComponent();
            this.chart1.Size = new System.Drawing.Size(this.Width-2* Convert.ToInt32(this.chart1.Left*ScaleX), Convert.ToInt32(330 * ScaleY));

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

            ShowDialog();

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

                    try
                    {
                        chart1.ChartAreas[0].RecalculateAxesScale();
                    }
                    catch (Exception ex)
                    {
                        chart1.ChartAreas[0].AxisY.IsLogarithmic = false;
                        System.Windows.Forms.MessageBox.Show("Zero or negative values cannot be displayed on a logarithmic scale");
                        chart1.ChartAreas[0].RecalculateAxesScale();
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
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
        }

        private void UpdateGraphBox()
        {
            int i = graphBox.SelectedIndex;
            graphBox.Items.Clear();
            graphBox.Text = "";
            if (outSeg == null) return;

            foreach (TGraphSetup TGS in outSeg.Graphs.GList)
            {
                graphBox.Items.Add(TGS.GraphName);
            }

            if ((i > -1) && (graphBox.Items.Count > i)) graphBox.SelectedIndex = i;
            if ((graphBox.SelectedIndex < 0) && (graphBox.Items.Count > 0)) graphBox.SelectedIndex = 0;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.DoEvents();

            aQTS.SavedRuns.TryGetValue(OutputBox.Text, out outSeg);

            UpdateGraphBox();
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
            chart1.Series.Clear();
            int sercnt = 0;
            unZoom();

            if (graphBox.SelectedIndex < 0) return;

            outSeg.SetMemLocRec();
            TGraphSetup Graph = outSeg.Graphs.GList[graphBox.SelectedIndex];

            foreach (SeriesID SID in Graph.YItems)
            {
                TStateVariable TSV = outSeg.GetStatePointer(SID.ns, SID.typ, SID.lyr);
                if (TSV != null)
                {
                    List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                    Series ser = chart1.Series.Add(SID.nm);

                    ser.ChartType = SeriesChartType.Line;
                    ser.BorderWidth = 2;
                    ser.MarkerStyle = MarkerStyle.Diamond;
                    ser.Enabled = true;
                    sercnt++;

                    for (int i = 0; i < TSV.SVoutput.Data.Keys.Count; i++)
                    {
                        ITimeSeriesOutput ito = TSV.SVoutput;
                        string datestr = ito.Data.Keys.ElementAt(i).ToString();
                        Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[SID.indx - 1]);
                        ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                    }
                }
            }
        }

        private void NewGraphButton_Click(object sender, EventArgs e)
        {
            if (outSeg == null) return;
            GraphSetupForm GSForm = new GraphSetupForm();
            if (GSForm.EditGraph(outSeg, -1)) UpdateGraphBox();
            graphBox.SelectedIndex = graphBox.Items.Count - 1;

        }

        private void graphBox_selectedIndexChange(object sender, EventArgs e)
        {
            DisplayGraph();
        }

        private void EditGraphButton_Click(object sender, EventArgs e)
        {
            if (outSeg == null) return;
            GraphSetupForm GSForm = new GraphSetupForm();
            if (GSForm.EditGraph(outSeg, graphBox.SelectedIndex)) UpdateGraphBox();
        }

        private void DeleteGraphButton_Click(object sender, EventArgs e)
        {
            if (graphBox.SelectedIndex < 0) return;
            if (MessageBox.Show("Erase the graph '" + graphBox.Text + "'?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                outSeg.Graphs.DeleteGraph(graphBox.SelectedIndex);
                UpdateGraphBox();
                DisplayGraph();
            }
        }

        private void DelRunButton_Click(object sender, EventArgs e)
        {
            if (OutputBox.SelectedIndex < 0) return;
            if (MessageBox.Show("Erase the archived output '" + OutputBox.Text + "'?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                aQTS.SavedRuns.Remove(OutputBox.Text);
                OutputBox.Items.RemoveAt(OutputBox.SelectedIndex);
                if (OutputBox.SelectedIndex < 0) { OutputBox.Text = ""; outSeg = null; }

                UpdateGraphBox();
                DisplayGraph();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (OutputBox.SelectedIndex < 0) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV File|*.CSV";
            saveFileDialog1.Title = "Save to Comma-Separated Text";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                string CSVstring = outSeg.ResultsToCSV();
                File.WriteAllText(saveFileDialog1.FileName, CSVstring);
            }
        }

        private void toggleLog_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.IsLogarithmic = !(chart1.ChartAreas[0].AxisY.IsLogarithmic);
        }

        private void unZoom()
        {
            chart1.ChartAreas [0].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
        }

        private void resetZoom_Click(object sender, EventArgs e)
        {
            unZoom();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            AQTTestForm.OpenUrl(HelpTopic);
        }
    }
}
