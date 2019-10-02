using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AQUATOX.AQTSegment;
using Data;

namespace GUI.AQUATOX
{
    public partial class AQTTestForm : Form
    {

        public AQTSim aQTS = null;

        public AQTTestForm()
        {
           InitializeComponent();
        }

        private void AQTTestForm_Load(object sender, EventArgs e)
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

//            chart1.Series.Clear();
            int sercnt = 0;

            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
            {
                int cnt = 0;
//                Series ser = chart1.Series.Add(TSV.PName);
                outtxt = outtxt + TSV.PName + ", ";

//                ser.ChartType = SeriesChartType.Line;
//                ser.BorderWidth = 2;
//                ser.MarkerStyle = MarkerStyle.Diamond;
//                ser.Enabled = false;
                sercnt++;

                SuppressText = (TSV.output.Data.Keys.Count > 5000);
                for (int i = 0; i < TSV.output.Data.Keys.Count; i++)
                {
                    ITimeSeriesOutput ito = TSV.output;
                    string datestr = ito.Data.Keys.ElementAt(i).ToString();
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
//                    ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                    cnt++;
                }

            }

            outtxt = outtxt + Environment.NewLine;

            //foreach (double Vals in TSV.Results)
            //{
            //    outtxt = outtxt + Vals + ", ";
            //    ser.Points.AddXY(aQTS.AQTSeg.SV.restimes[cnt], Vals);
            //    cnt++;
            //}
            //outtxt = outtxt + Environment.NewLine;
            //outtxt = outtxt + Environment.NewLine;

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
            if (aQTS == null) textBox1.Text = "Simulation not Instantiated";
            else
            {
                string errmessage = aQTS.Integrate();
                if (errmessage == "") DisplaySVs();
                else textBox1.Text = errmessage;
            }
        }
    }
}
