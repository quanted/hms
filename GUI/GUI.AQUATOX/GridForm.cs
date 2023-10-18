using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class GridForm : Form
    {
        public bool gridChange;
        bool UserCanceled;
        string HelpTopic = "";
        bool selectinglake = false;
        public string chosenlake = "";
        public string chosenfileN = "";

        public GridForm()
        {
            InitializeComponent();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public bool ShowGrid(DataTable table, bool LockLeftColumn, bool addrows, string HelpContext)
        {
            HelpTopic = HelpContext;
            gridChange = false;
            UserCanceled = false;
            dataGridView1.DataSource = null;  // 11/29/2021
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].ReadOnly = LockLeftColumn;
            dataGridView1.Columns[0].Frozen = true;
            dataGridView1.AllowUserToAddRows = addrows;
            ShowDialog();
            return !UserCanceled;
        }

        private void AutosizeCol(int col)
        {
            dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[col].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        public bool SelectRow(DataTable table, string HelpContext)
        {
            HelpTopic = HelpContext;
            gridChange = false;
            UserCanceled = false;
            dataGridView1.DataSource = null;  // 11/29/2021
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].Frozen = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ReadOnly = true;

            AutosizeCol(0);
            AutosizeCol(2);
            AutosizeCol(3);
            AutosizeCol(4);
            AutosizeCol(5);
            AutosizeCol(6);
            Width = 1400; Height = 800;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            selectinglake = true;
            ShowDialog();
            return !UserCanceled;
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
            chosenlake = "";
            chosenfileN = "";

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
            AQTTestForm.OpenUrl(HelpTopic);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!selectinglake) return;
            if (e.RowIndex < 0) return;
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            chosenlake = row.Cells[0].Value.ToString();
            chosenfileN = row.Cells[6].Value.ToString();
        }

        private void copybutton_Click(object sender, EventArgs e)
        {
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
    }
}
