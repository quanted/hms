using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class DataFrequency : Form
    {
        public string dataFrequency { get; set; }

        public DataFrequency()
        {
            InitializeComponent();
        }

        private void bttnFreqSelect_Click(object sender, EventArgs e)
        {
            dataFrequency = cmbBxFrequency.SelectedItem.ToString();
            this.Close();
        }
    }
}
