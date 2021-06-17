using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class CheckboxForm : Form
    {
        public CheckboxForm()
        {
            InitializeComponent();
        }

        public List<bool> SelectFromBoxes(List<string> InList)
        {
            checkedListBox1.DataSource = InList;
            if (ShowDialog()== DialogResult.Cancel) return null;
            List<bool> retlist = new List<bool>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                retlist.Add(checkedListBox1.GetItemChecked(i));
            return retlist;
        }
                
    }
}
