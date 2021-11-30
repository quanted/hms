using System;
using System.Data;
using System.Windows.Forms;
using AQUATOX.AQTSegment;

namespace GUI.AQUATOX
{
    public partial class TrophMatrix : Form
    {
        public bool gridChange;
        bool UserCanceled;
        AQUATOXSegment AQS = null;
        DataTable[] tbl;
        int iTable = 0;

        public TrophMatrix()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Return the minimum width in pixels a DataGridView can be before the control's vertical scrollbar would be displayed.
        /// </summary>
        private int GetDgvMinWidth(DataGridView dgv)
        {
            // Add two pixels for the border for BorderStyles other than None.
            var controlBorderWidth = (dgv.BorderStyle == BorderStyle.None) ? 0 : 2;

            // Return the width of all columns plus the row header, and adjusted for the DGV's BorderStyle.
            return dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dgv.RowHeadersWidth + controlBorderWidth;
        }

        public bool ShowGrid(DataTable[] table, AQUATOXSegment AQTS)
        {
            AQS = AQTS;
            tbl = table;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = table[0];
            gridChange = false;
            UserCanceled = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.Width = 80;
                col.SortMode = DataGridViewColumnSortMode.NotSortable; 
            }

            dataGridView1.Columns[0].Width = 180;
            dataGridView1.Columns[0].Frozen = true;


            this.Width = GetDgvMinWidth(dataGridView1) + 100;
            int waw = Screen.GetWorkingArea(this).Width;
            if (this.Width > waw) this.Width = waw;

            ShowDialog();
            return !UserCanceled;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            if (!gridChange)
            {
                UserCanceled = true;
                this.Close();
            }
            else //grid has changed, get confirmation first
                if (MessageBox.Show("Cancel your changes to the data matrix?", "Confirm",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                     MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    UserCanceled = true;
                    this.Close();
                }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            gridChange = true;
            changedLabel.Visible = true;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        private void RenormalizeButton_Click(object sender, EventArgs e)
        {
            AQS.Normalize_Trophint_Table(ref tbl[0]);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = tbl[0];
            ToggleLabel.Text = "Showing Feeding Preferences";
        }

        private void toggle_button_click(object sender, EventArgs e)
        {
            iTable++;
            if (iTable == 3) iTable = 0;
            if (iTable == 0) ToggleLabel.Text = "Showing Feeding Preferences";
              else if (iTable == 1) ToggleLabel.Text = "Showing Egestion Coefficients";
              else ToggleLabel.Text = "Showing Comments/References";
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = tbl[iTable];

        }
    }
}
