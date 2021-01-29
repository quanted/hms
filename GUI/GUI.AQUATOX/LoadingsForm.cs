using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AQUATOX.Animals;
using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using AQUATOX.Plants;
using AQUATOX.Loadings;
using Globals;

namespace GUI.AQUATOX
{
    public partial class LoadingsForm : Form
    {
        public TStateVariable SV;
        
        public LoadingsForm()
        {
            InitializeComponent();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {

        }

        public bool EditSV(TStateVariable IncomingS, AQTSim AQS)
        {

            string backup = Newtonsoft.Json.JsonConvert.SerializeObject(IncomingS, AQS.AQTJSONSettings()); ;

            SV = IncomingS;
            UpdateScreen();

            if (ShowDialog() != DialogResult.Cancel)
            {
                IncomingS = Newtonsoft.Json.JsonConvert.DeserializeObject<TStateVariable>(backup, AQS.AQTJSONSettings());
                return false;
              }
            else return true;

        }

        public bool Has_Alt_Loadings()
        {
            return (SV.Layer == T_SVLayer.WaterCol) &&
            ((SV.NState == AllVariables.Volume) || (SV.NState == AllVariables.H2OTox) || (SV.NState == AllVariables.Phosphate)
            || (SV.NState == AllVariables.Oxygen) || (SV.NState == AllVariables.Ammonia) || (SV.NState == AllVariables.Nitrate));

            //  Salinity, DissRefrDetr..LastDetr, FirstAnimal..LastFish]) and (Typ = StV))  FIXME
        }
            

        public void UpdateScreen()
        {
            // Future, add button for frac avail, trophic matrix, biotransformation
            // Future add photoperiod edit for light inputs
            // Future add TSS TSSolids vs TSSediment
            // Future add LInk Inflow/outflow warnings?
            // Future manage Detritus Inputs
            // Future CO2 Equilibrium button
            // Future Exposure Inputs

            SV.UpdateUnits();
            ICUnit.Text = SV.StateUnit;
            CLUnit.Text = SV.LoadingUnit;
            ParameterButton.Visible = ((SV.IsPlant()) || (SV.IsAnimal()) || (SV.NState == AllVariables.H2OTox));

            AmmoniaDriveLabel.Visible = (SV.NState == AllVariables.Ammonia) && (SV.AQTSeg.PSetup.AmmoniaIsDriving.Val);
        
            NotesEdit.Text = SV.LoadNotes1;
            NotesEdit2.Text = SV.LoadNotes2;

            SVNameLabel.Text = SV.PName;
            ICEdit.Text = SV.InitialCond.ToString("G9");

            IgnoreLoadingsBox.Checked = SV.LoadsRec.Loadings.NoUserLoad;
            LoadingsPanel.Visible = !SV.LoadsRec.Loadings.NoUserLoad;

            if (!SV.LoadsRec.Loadings.NoUserLoad) {
                LTBox.Items.Clear();
                LTBox.Items.Add("Inflow Water");
                if (Has_Alt_Loadings())
                {
                    LTBox.Items.Add("Point Source");
                    LTBox.Items.Add("Direct. Precip.");
                    LTBox.Items.Add("Non-Point Source");
                }
                LTBox.SelectedIndex = 0;
                ShowGrid();
            }


        }

        public void ShowGrid()
        {

            TLoadings LoadShown;
            if (LTBox.SelectedIndex<1) LoadShown = SV.LoadsRec.Loadings; 
            else LoadShown = SV.LoadsRec.Alt_Loadings[LTBox.SelectedIndex - 1];

            UseConstRadio.Checked = LoadShown.UseConstant;
            UseTimeSeriesRadio.Checked = !LoadShown.UseConstant;
            dataGridView1.Enabled = !LoadShown.UseConstant;
            ConstLoadBox.Enabled = LoadShown.UseConstant;
            CLUnit.Enabled = LoadShown.UseConstant;

            ConstLoadBox.Text = LoadShown.ConstLoad.ToString("G9");
            MultLoadBox.Text = LoadShown.MultLdg.ToString("G9");

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

            for (int i=0; i<LoadShown.list.Count; i++)
            {
                DataRow row = LoadTable.NewRow();
                row[0] = (LoadShown.list.Keys[i]);
                row[1] = (LoadShown.list.Values[i]);
                LoadTable.Rows.Add(row);
            }

            dataGridView1.DataSource = LoadTable;
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void ParameterButton_Click(object sender, EventArgs e)
        {
            if (SV.IsPlant())
            {
                TPlant TP = SV as TPlant;
                Param_Form plantform = new Param_Form();
                PlantRecord PIR = TP.PAlgalRec;
                PIR.Setup();
                TParameter[] PPS = PIR.InputArray();
                plantform.EditParams(ref PPS, "Plant Parameters", false);
            }

            if (SV.IsAnimal())
            {
                TAnimal TA = SV as TAnimal;
                Param_Form animform = new Param_Form();
                AnimalRecord AIR = TA.PAnimalData;
                AIR.Setup();
                TParameter[] PPS = AIR.InputArray();
                animform.EditParams(ref PPS, "Animal Parameters", false);
            }

            if (SV.NState == AllVariables.H2OTox)
            {
                TToxics TC = SV as TToxics;
                Param_Form chemform = new Param_Form();
                ChemicalRecord CR = TC.ChemRec; CR.Setup();
                TParameter[] PPS = CR.InputArray();
                chemform.EditParams(ref PPS, "Chem Parameters", false);
            }

        }

        private void ButtonPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IgnoreLoadingsBox_CheckedChanged(object sender, EventArgs e)
        {
            SV.LoadsRec.Loadings.NoUserLoad = IgnoreLoadingsBox.Checked;
            LoadingsPanel.Visible = !SV.LoadsRec.Loadings.NoUserLoad;
            UpdateScreen();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LTBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowGrid();
        }

        private void NotesEdit_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
