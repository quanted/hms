using System;
using System.Data;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class TrophMatrix : Form
    {
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

        public bool ShowGrid(DataTable table)
        {
            dataGridView1.DataSource = table;

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

            Show();
            return true;
        }

    }
}
