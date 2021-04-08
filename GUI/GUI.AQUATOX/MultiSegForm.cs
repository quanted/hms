using Globals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class MultiSegForm : Form
    {

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that 
        /// window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        private const int WM_SETREDRAW = 11;

    
        public MultiSegForm()
        {
            AutoScroll = true;
            InitializeComponent();
        }

    
        private void cancel_click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel any changes to inputs?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                // UserCanceled = true;
                this.Close();
            }
        }

        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
