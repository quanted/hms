using Globals;
using System;
using System.Data;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class ChemToxForm : Form
    {
        public bool gridChange;
        bool UserCanceled;
        string HelpTopic = "ChemTox";
        UptakeCalcMethodType LocalAM;
        UptakeCalcMethodType LocalPM;

        public ChemToxForm()
        {
            InitializeComponent();
        }



        public void SetupDataGridView(DataGridView dgv, DataTable dt)
        {
            dgv.DataSource = null;  // 11/29/2021
            dgv.DataSource = dt;
            dgv.Columns[0].ReadOnly = false; // LockLeftColumn; 
            dgv.Columns[0].Frozen = true;
            dgv.AllowUserToAddRows = true; //  addrows; 
        }

        public bool ShowGrids(DataTable ATox, DataTable PTox, ref UptakeCalcMethodType AMethod, ref UptakeCalcMethodType PMethod )
        {
            LocalAM = AMethod; LocalPM = PMethod;
            HelpTopic = "ChemTox";
            gridChange = false;
            UserCanceled = false;
            SetupRadioButtons();
            SetupDataGridView(dataGridView1, ATox);
            SetupDataGridView(dataGridView2, PTox);
            ShowDialog();
            if (!UserCanceled) { AMethod = LocalAM; PMethod = LocalPM; }
            return !UserCanceled;
        }

        private void SetupRadioButtons()
        {
            AK2Only.Checked = (LocalAM == UptakeCalcMethodType.Default_Meth);
            ACalcK1.Checked = (LocalAM == UptakeCalcMethodType.CalcK1);
            ACalcK2.Checked = (LocalAM == UptakeCalcMethodType.CalcK2);
            ACalcBCF.Checked = (LocalAM == UptakeCalcMethodType.CalcBCF);

            PK2Only.Checked = (LocalPM == UptakeCalcMethodType.Default_Meth);
            PCalcK1.Checked = (LocalPM == UptakeCalcMethodType.CalcK1);
            PCalcK2.Checked = (LocalPM == UptakeCalcMethodType.CalcK2);
            PCalcBCF.Checked = (LocalPM == UptakeCalcMethodType.CalcBCF);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            gridChange = true;
            changedLabel.Visible = true;

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            if (gridChange)
            {
                if (MessageBox.Show("Cancel your changes to the data matrix?", "Confirm",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                     MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    UserCanceled = true;
                    this.Close();
                }
            }
            else
            {
                UserCanceled = true;
                this.Close();
            }

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            {
                if ((e.Context & DataGridViewDataErrorContexts.Parsing) == DataGridViewDataErrorContexts.Parsing)
                {
                    MessageBox.Show("Wrong data type entered.");
                }

                if ((e.Exception) is ConstraintException)
                {
                    MessageBox.Show(e.Exception.Message);
                }

                e.ThrowException = false;

            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            AQTMainForm.OpenUrl(HelpTopic);
        }

        private void RB_Click(object sender, EventArgs e)
        {
            if (AK2Only.Checked) LocalAM = UptakeCalcMethodType.Default_Meth;
            if (ACalcK1.Checked) LocalAM = UptakeCalcMethodType.CalcK1;
            if (ACalcK2.Checked) LocalAM = UptakeCalcMethodType.CalcK2;
            if (ACalcBCF.Checked) LocalAM = UptakeCalcMethodType.CalcBCF;

            if (PK2Only.Checked) LocalPM = UptakeCalcMethodType.Default_Meth;
            if (PCalcK1.Checked) LocalPM = UptakeCalcMethodType.CalcK1;
            if (PCalcK2.Checked) LocalPM = UptakeCalcMethodType.CalcK2;
            if (PCalcBCF.Checked) LocalPM = UptakeCalcMethodType.CalcBCF;
        }
    }
}
