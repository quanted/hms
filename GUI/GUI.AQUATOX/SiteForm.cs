using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Loadings;
using Globals;
using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
 
namespace GUI.AQUATOX
{

    public partial class SiteForm : Form
    {
        private bool GridChanged = false;
        private AQTSim Sim = null;

        public SiteForm()
        {
            InitializeComponent();
        }

        public bool EditSiteInfo(AQTSim AQS)
        {

            Sim = AQS;
            string backup = Newtonsoft.Json.JsonConvert.SerializeObject(AQS.AQTSeg, AQS.AQTJSONSettings());

            UpdateScreen();

            if (ShowDialog() == DialogResult.Cancel)
            {
                Newtonsoft.Json.JsonConvert.PopulateObject(backup, AQS.AQTSeg, AQS.AQTJSONSettings());
                return false;
            }
            else return true;

        }


        public void UpdateScreen()
        {
            RBPanel.Visible = true;
            ParameterButton.Visible = true;
            LoadingsPanel.Visible = true;

            LTBox.DataSource = new List<string>(new string[] { "Velocity (cm/s)", "Mean Depth (m)", "Evaporation", "Shade (fraction)" });  //special case 
            LTBox.SelectedIndex = 0;

            if  (Sim.AQTSeg.Location.SiteType == SiteTypes.Pond) RBPond.Checked = true; 
            else if (Sim.AQTSeg.Location.SiteType == SiteTypes.Lake) RBLake.Checked = true;
            else if (Sim.AQTSeg.Location.SiteType == SiteTypes.Stream) RBStream.Checked = true;
            else if (Sim.AQTSeg.Location.SiteType == SiteTypes.Reservr1D) RBRes.Checked = true;
            else if (Sim.AQTSeg.Location.SiteType == SiteTypes.Enclosure) RBEncl.Checked = true;
            else if (Sim.AQTSeg.Location.SiteType == SiteTypes.Marine) RBMarine.Checked = true;

            ShowGrid();
        }

        TLoadings LoadShown;

        public void ShowGrid()
        {
            GridChanged = false;

            if (LTBox.SelectedIndex == 0)
            {
                if (Sim.AQTSeg.DynVelocity == null) Sim.AQTSeg.DynVelocity = new TLoadings();
                LoadShown = Sim.AQTSeg.DynVelocity;
                UseConstRadio.Text = "Calculate Velocity (using flow)";
                UseTimeSeriesRadio.Text = "Use Time Series Below (cm/s)";
                UseConstRadio.Checked = Sim.AQTSeg.CalcVelocity;
                UseTimeSeriesRadio.Checked = !Sim.AQTSeg.CalcVelocity;

                ConstLoadBox.Visible = false;
            }

            if (LTBox.SelectedIndex == 1)
            {
                if (Sim.AQTSeg.DynZMean == null) Sim.AQTSeg.DynZMean = new TLoadings();
                LoadShown = Sim.AQTSeg.DynZMean;
                UseConstRadio.Text = "Use Constant (m)";
                UseTimeSeriesRadio.Text = "Use Time Series Below (m)";
                UseConstRadio.Checked = Sim.AQTSeg.UseConstZMean;
                UseTimeSeriesRadio.Checked = !Sim.AQTSeg.UseConstZMean;

                ConstLoadBox.Visible = true;
                ConstLoadBox.Text = Sim.AQTSeg.Location.Locale.ICZMean.Val.ToString("G9");
            }

            if (LTBox.SelectedIndex == 2)
            {
                if (Sim.AQTSeg.DynEvap == null) Sim.AQTSeg.DynEvap = new TLoadings();
                LoadShown = Sim.AQTSeg.DynEvap;
                UseConstRadio.Text = "Use Constant (inch/yr)";
                UseTimeSeriesRadio.Text = "Use Time Series Below (m3/d)";
                UseConstRadio.Checked = Sim.AQTSeg.UseConstEvap;
                UseTimeSeriesRadio.Checked = !Sim.AQTSeg.UseConstEvap;

                ConstLoadBox.Visible = true;
                ConstLoadBox.Text = Sim.AQTSeg.Location.Locale.MeanEvap.Val.ToString("G9");
            }

            if (LTBox.SelectedIndex == 3)
            {
                if (Sim.AQTSeg.Shade.Loadings == null) Sim.AQTSeg.Shade.Loadings = new TLoadings();
                LoadShown = Sim.AQTSeg.Shade.Loadings;
                UseConstRadio.Text = "Use Constant (frac)";
                UseTimeSeriesRadio.Text = "Use Time Series Below (frac canopy)";
                UseConstRadio.Checked = LoadShown.UseConstant;
                UseTimeSeriesRadio.Checked = !LoadShown.UseConstant;

                ConstLoadBox.Visible = true;
                ConstLoadBox.Text = LoadShown.ConstLoad.ToString("G9");
            }

            dataGridView1.Enabled = UseTimeSeriesRadio.Checked;

            DataTable LoadTable = new DataTable("Loadings");
            DataColumn datecolumn = new DataColumn();
            datecolumn.Unique = true;
            datecolumn.ColumnName = "Date";
            datecolumn.DataType = System.Type.GetType("System.DateTime");
            LoadTable.Columns.Add(datecolumn);

            DataColumn loadcolumn = new DataColumn();
            loadcolumn.Unique = false;
            loadcolumn.ColumnName = "Loading";
            loadcolumn.DataType = System.Type.GetType("System.Double");
            LoadTable.Columns.Add(loadcolumn);

            for (int i = 0; i < LoadShown.list.Count; i++)
            {
                DataRow row = LoadTable.NewRow();
                row[0] = (LoadShown.list.Keys[i]);
                row[1] = (LoadShown.list.Values[i]);
                LoadTable.Rows.Add(row);
            }

            dataGridView1.DataSource = LoadTable;
            if (LoadShown.UseConstant) dataGridView1.ForeColor = Color.Gray;
            else dataGridView1.ForeColor = Color.Black;
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void ParameterButton_Click(object sender, EventArgs e)
        {
            Param_Form Siteform = new Param_Form();
            SiteRecord SR = Sim.AQTSeg.Location.Locale;
            SR.Setup();
            TParameter[] PPS = SR.InputArray();
            Siteform.EditParams(ref PPS, "Site Parameters", false, "SiteLib.JSON");
            
            UpdateScreen();
        }


        private void OKButton_Click(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();
            this.Close();
        }

        private void LTBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();
            ShowGrid();
        }



        private void File_Import_Click(object sender, EventArgs e)
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv files(*.csv)|*.csv|tab delimited txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    LoadShown.list.Clear();

                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    string[] csvRows = System.IO.File.ReadAllLines(filePath, Encoding.Default);
                    int errcount = 0;

                    foreach (var row in csvRows)
                    {
                        var columns = row.Split(',');

                        string field1 = columns[0];
                        string field2 = columns[1];

                        try
                        {
                            LoadShown.list.Add(DateTime.Parse(field1), double.Parse(field2));
                        }
                        catch
                        {
                            errcount++;
                            if (errcount > 1)  // header line error is OK
                            {
                                MessageBox.Show("Unexpected format.  A two column input file with [DATE, LOADING] expected.", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return;
                            }
                        }
                    }

                    ShowGrid();
                }
            }
        }

        private void UseConstRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();

            if (LTBox.SelectedIndex == 0) Sim.AQTSeg.CalcVelocity = UseConstRadio.Checked;
            if (LTBox.SelectedIndex == 1) Sim.AQTSeg.UseConstZMean = UseConstRadio.Checked;
            if (LTBox.SelectedIndex == 2) Sim.AQTSeg.UseConstEvap = UseConstRadio.Checked;
            if (LTBox.SelectedIndex == 3) LoadShown.UseConstant = UseConstRadio.Checked;

            ShowGrid();
        }

        private void SaveGrid()
        {
            DataTable LoadTable = dataGridView1.DataSource as DataTable;
            LoadShown.list.Clear();
            LoadShown.list.Capacity = LoadTable.Rows.Count;
            for (int i = 0; i < LoadTable.Rows.Count; i++)
            {
                DataRow row = LoadTable.Rows[i];
                LoadShown.list.Add(row.Field<DateTime>(0), row.Field<double>(1));
            }
        }


        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            {
                if ((e.Context & DataGridViewDataErrorContexts.Parsing) == DataGridViewDataErrorContexts.Parsing)
                    MessageBox.Show("Wrong data type entered.");

                if ((e.Exception) is ConstraintException)
                    MessageBox.Show(e.Exception.Message);

                e.ThrowException = false;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridChanged = true;
        }


        private void RB_Changed(object sender, EventArgs e)
        {
            if (RBPond.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Pond; 
            else if (RBLake.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Lake;
            else if (RBStream.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Stream;
            else if (RBRes.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Reservr1D;
            else if (RBEncl.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Enclosure;
            else if (RBMarine.Checked) Sim.AQTSeg.Location.SiteType = SiteTypes.Marine;
        }

        private void ConstLoadBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double val = double.Parse(ConstLoadBox.Text);
                if (LTBox.SelectedIndex == 1) Sim.AQTSeg.Location.Locale.ICZMean.Val = val;
                if (LTBox.SelectedIndex == 2) Sim.AQTSeg.Location.Locale.MeanEvap.Val = val;  
                if (LTBox.SelectedIndex == 3) LoadShown.ConstLoad = val;
                ConstLoadBox.BackColor = Color.White;
            }
            catch
            {
                ConstLoadBox.BackColor = Color.Yellow;
            }
        }

    }
}
