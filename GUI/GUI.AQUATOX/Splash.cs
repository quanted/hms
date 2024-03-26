using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GUI.AQUATOX
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            this.ControlBox = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }


        private void SingleSeg_Click(object sender, EventArgs e)
        {
            try
            {
                AQTMainForm AQTForm = new AQTMainForm();
                AQTForm.FormClosed += Program.OnFormClosed;
                AQTForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Single segment mode exception: " + ex.Message);
            }
        }

        private void MultiSeg_Click(object sender, EventArgs e)
        {
            try
            {
                MultiSegForm MSForm = new MultiSegForm();
                MSForm.FormClosed += Program.OnFormClosed;
                MSForm.HAWQS_apikey = Properties.Settings.Default.HAWQS_apikey;
                MSForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Multi-segment mode exception: " + ex.Message);
            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
