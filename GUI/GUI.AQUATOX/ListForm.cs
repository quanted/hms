using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class ListForm : Form
    {
        public ListForm()
        {
            InitializeComponent();
        }

        public int SelectFromList(List<string> InList)
        {
            listBox1.DataSource = null;
            listBox1.DataSource = InList;
            if (ShowDialog()== DialogResult.Cancel) return -1;
            return listBox1.SelectedIndex;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
