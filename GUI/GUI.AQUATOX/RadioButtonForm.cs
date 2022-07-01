using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class RadioButtonForm : Form
    {
        public string selectedString;

        public RadioButtonForm()
        {
            InitializeComponent();
        }

        public RadioButtonForm(IList<string> lst)
        {
            InitializeComponent();
            for (int i = 0; i < lst.Count; i++)
            {
                RadioButton rdb = new RadioButton();
                rdb.Text = lst[i];
                rdb.Size = new Size(220, 30);
                this.Controls.Add(rdb);
                rdb.Location = new Point(40, 70 + 35 * i);
                rdb.CheckedChanged += (s, ee) =>
                {
                    var r = s as RadioButton;
                    if (r.Checked)
                        this.selectedString = r.Text;
                };
                if (i==0) rdb.Checked = true;
                
            }
            this.Height = lst.Count * 35 + 130;
        }



    }
}
