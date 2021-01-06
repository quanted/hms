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

            AmmoniaDriveLabel.Visible = (SV.NState == AllVariables.Ammonia) || (SV.AQTSeg.PSetup.AmmoniaIsDriving.Val);

            NotesEdit.Text = SV.LoadNotes1;
            NotesEdit2.Text = SV.LoadNotes2;
            IgnoreLoadingsBox.Checked = SV.LoadsRec.Loadings.NoUserLoad;
            SVNameLabel.Text = SV.PName;
            ICEdit.Text = SV.InitialCond.ToString("G9");  


        }

        public bool ShowGrid(DataTable table)
        {
            dataGridView1.DataSource = table;
            Show();
            return true;
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {

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

    }
}
