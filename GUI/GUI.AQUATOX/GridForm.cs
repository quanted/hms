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

        public bool ShowGrid(DataTable table, bool LockLeftColumn, bool addrows)
        {
            gridChange = false;
            UserCanceled = false;
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].ReadOnly = LockLeftColumn;
            dataGridView1.Columns[0].Frozen = true;
            dataGridView1.AllowUserToAddRows = addrows;
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
            if (gridChange)
                if (MessageBox.Show("Cancel your changes to the data matrix?", "Confirm",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                     MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
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

        
    }
}
