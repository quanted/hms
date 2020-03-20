using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using AQUATOX.AQTSegment;
using Data;
using System.ComponentModel;
using Web.Services.Models;
using Web.Services.Controllers;
using System.Collections.Generic;

namespace GUI.AQUATOX.Workflow
{
    /// <summary>
    /// WaterQuality Input that implements TimeSeriesInput object.
    /// </summary>
    public class WaterQualityInput
    {
        /// <summary>
        /// Specified dataset for the workflow
        /// </summary>
        //[Required]
        //public List<List<string>> ConnectivityTable { get; set; }     //To load from file, will be added at a later date.

        /// <summary>
        /// TaskID required for data storage in mongodb
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// Data source for data retrieval
        /// If value is 'nldas': surface runoff and subsurface flow will be from nldas (no precip will be downloaded); 
        /// If value is 'ncei', precip data will be downloaded from the closest station to the catchment and curvenumber will be used for surface runoff/subsurface flow.
        /// </summary>
        public string DataSource { get; set; }

        public int MinNitrate { get; set; }
        public int MaxNitrate { get; set; }
        public int MinAmmonia { get; set; }
        public int MaxAmmonia { get; set; }

    }

    public partial class AQTWorkflowForm : Form
    {
        private Chart chart1 = new Chart();
        private BackgroundWorker Worker = new BackgroundWorker();
        private string errmessage;

        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        Series series1 = new Series();
        
        public AQTSim aQTS = null;

        public AQTWorkflowForm()
        {
            InitializeComponent();
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunCompleted);
            Worker.WorkerReportsProgress = true;

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            SuspendLayout();

            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);

            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(31, 250);
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(809, 233);
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

        private void AQTWorkflowForm_Load(object sender, EventArgs e)
        {

        }

        public void DisplaySVs()

        {
            string outtxt = "Date, ";
            bool SuppressText = true;

            //outtxt = outtxt + "Times: ";
            //foreach (DateTime Times in aQTS.AQTSeg.SV.restimes)
            //    outtxt = outtxt + Times + ", ";
            //outtxt = outtxt + Environment.NewLine;

            chart1.Series.Clear();
            int sercnt = 0;

            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
            {
                int cnt = 0;
                Series ser = chart1.Series.Add(TSV.PName);
                outtxt = outtxt + TSV.PName.Replace(",", "") + ", ";  // suppress commas in name for CSV output

                ser.ChartType = SeriesChartType.Line;
                ser.BorderWidth = 2;
                ser.MarkerStyle = MarkerStyle.Diamond;
                ser.Enabled = false;
                sercnt++;

                SuppressText = (TSV.output.Data.Keys.Count > 5000);
                for (int i = 0; i < TSV.output.Data.Keys.Count; i++)
                {
                    ITimeSeriesOutput ito = TSV.output;
                    string datestr = ito.Data.Keys.ElementAt(i).ToString();
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                    ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                    cnt++;
                }

            }

            outtxt = outtxt + Environment.NewLine;

            if (!SuppressText)
            {
                TStateVariable TSV1 = aQTS.AQTSeg.SV[0];
                for (int i = 0; i < TSV1.output.Data.Keys.Count; i++)
                {
                    bool writedate = true;
                    foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                    {
                        ITimeSeriesOutput ito = TSV.output;
                        if (writedate)
                        {
                            string datestr = ito.Data.Keys.ElementAt(i).ToString();
                            outtxt = outtxt + datestr + ", ";
                            writedate = false;
                        }
                        Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                        outtxt = outtxt + Val.ToString() + ", ";
                    }
                    outtxt = outtxt + Environment.NewLine;
                }
            }

            textBox1.Text = outtxt;
        }


        private void loadJSON_Click(object sender, EventArgs e)
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
                string jsondata = "";
                string errmessage = aQTS.SaveJSON(ref jsondata);
                if (errmessage == "") File.WriteAllText(saveFileDialog1.FileName, jsondata);
                else textBox1.Text = errmessage;
            }
        }

        private void integrate_Click(object sender, EventArgs e)
        {
            WSWatershedWorkFlow workFlow = new WSWatershedWorkFlow();
            WatershedWorkflowInput workflowInput = new WatershedWorkflowInput();  //add examples
            workFlow.GetWorkFlowData(workflowInput);         // 
            
            //results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            // return new ObjectResult(results);


            //if (aQTS == null) textBox1.Text = "Simulation not Instantiated";
            //else
            //{
            //    progressBar1.Visible = true;
            //    Worker.RunWorkerAsync();
            //}
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            aQTS.AQTSeg.ProgWorker = worker;
            errmessage = aQTS.Integrate();

       }

       private void Worker_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // progressBar1.Visible = false;

            progressBar1.Update();
            if (e.Error != null) textBox1.Text = "Error Raised: " + e.Error.Message;
            else if (errmessage == "")
            {
                textBox1.Text = "Run Completed.  Please wait one moment -- writing and plotting results";
                Application.DoEvents();
                DisplaySVs();
                progressBar1.Visible = false;
            }
            else textBox1.Text = errmessage;
        }


        // This event handler updates the progress bar
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100) progressBar1.Value = (e.ProgressPercentage+1);  // workaround of animation bug
            progressBar1.Value = (e.ProgressPercentage);
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


    }
}
