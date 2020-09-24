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
    public partial class Param_Form : Form
    {
        int Spacing;  // dense = 28; less dense 36
        int labelwidth = 210;
        int topbuffer = 5;

        public bool SuppressSymbol=false, SuppressComment=false;
        bool UserCanceled = false;
        int nparam;
        TParameter[] plist;

        TextBox[] edits;
        Label[] labels;
        Label[] units;
        TextBox[] references;
        DateTimePicker[] dateedits;
        CheckBox[] booledits;

        public Param_Form()
        {
            AutoScroll = true;
            InitializeComponent();
        }

        public void AddEdit(ref TParameter Param, int index)
        {
            string textstr;

            int top = topbuffer + (Spacing * index);

            if (Param is TBoolParam)
            {
                booledits[index] = new CheckBox();
                booledits[index].FlatAppearance.BorderSize = 2;
                booledits[index].Location = new Point(30,top+12);
                booledits[index].Size = new Size(360, 19);
                booledits[index].TabIndex = 0 + (2 * index);
                booledits[index].Text = Param.Name;
                booledits[index].Checked = ((TBoolParam)Param).Val;
                Controls.Add(booledits[index]);
                return;
            }

            labels[index] = new Label();
            if (Param is TSubheading)
            {
                labels[index].Text = ((TSubheading)Param).Val;
                labels[index].Location = new Point(12, top-4);
                labels[index].Size = new Size(360, 39);
                labels[index].Font = new Font(labels[index].Font.FontFamily, 12, FontStyle.Bold);
                labels[index].TextAlign = ContentAlignment.MiddleLeft;
                Controls.Add(labels[index]);
                return;
            }

            labels[index].Location = new Point(12, top);
            labels[index].Size = new Size(labelwidth, 39);
            labels[index].TextAlign = ContentAlignment.MiddleRight;
            if (SuppressSymbol) textstr = Param.Name;
                else textstr = Param.Symbol + " (" + Param.Name + ")";
            labels[index].Text = textstr;
            new ToolTip().SetToolTip(labels[index], textstr);
            Controls.Add(labels[index]);
            
            if (Param is TDateParam)
            {
                dateedits[index] = new DateTimePicker();
                dateedits[index].CustomFormat = "MM/dd/yyyy";
                dateedits[index].Format = DateTimePickerFormat.Custom; 
                dateedits[index].Location = new Point(labelwidth+22, top + 9);
                dateedits[index].Size = new Size(95, 23);
                dateedits[index].TabIndex = 0 + (2 * index);
                dateedits[index].Value = ((TDateParam)Param).Val;
                Controls.Add(dateedits[index]);
                return;
            }

            edits[index] = new TextBox();
            edits[index].Location = new Point(labelwidth + 22, top + 9);
            edits[index].Size = new Size(90, 23);
            edits[index].TabIndex = 0 + (2 * index);
            edits[index].Text = Param.Val.ToString();
            Controls.Add(edits[index]);

            units[index] = new Label();
            units[index].Location = new Point(labelwidth+115, top);
            units[index].Size = new Size(68, 39);
            units[index].Text = Param.Units;
            units[index].TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(units[index]);

            if (!SuppressComment)
                {
                    references[index] = new TextBox();
                    references[index].Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)| AnchorStyles.Right)));
                    references[index].Location = new Point(labelwidth+190, top + 9);
                    references[index].TabIndex = 1 + (2 * index);
                    references[index].Size = new Size(720-labelwidth, 23);
                    references[index].Text = Param.Comment;
                    Controls.Add(references[index]);
                 }
        }

        public void ReadEdit(ref TParameter Param, int index)
        {
            double f;
            if (Param is TSubheading) return;
            if (Param is TBoolParam) ((TBoolParam)Param).Val = booledits[index].Checked;
            else if (Param is TDateParam) ((TDateParam)Param).Val = dateedits[index].Value; 
            else if (double.TryParse(edits[index].Text, out f)) { Param.Val = f; };

            if (!SuppressComment)
                Param.Comment = references[index].Text;

        }


        private void Param_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (UserCanceled) return;

            for (int i = 0; i < nparam; i++)
            {
                ReadEdit(ref plist[i], i);
            }
        }

        private void cancel_click(object sender, EventArgs e)
        {
           if (MessageBox.Show("Cancel any changes to inputs?","Confirm" ,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                  UserCanceled = true;
                  this.Close();
                }
        }

        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool EditParams(ref TParameter[] parmlist, string Title, bool Dense)
        {
            if (Dense) Spacing = 28; else Spacing = 36;
            this.CancelButton = CancelButt;
            plist = parmlist;
            this.Text = Title;
            nparam = parmlist.Length;
            edits = new TextBox[nparam];
            labels = new Label[nparam]; 
            units = new Label[nparam]; 
            references = new TextBox[nparam];
            dateedits = new DateTimePicker[nparam];
            booledits = new CheckBox[nparam];

            for (int i=0; i<nparam; i++)
            {
                AddEdit(ref parmlist[i], i);
            }

            // Adjust height and width of box to be appropriate for number of edits
            this.Height = Spacing * nparam + 75;
            int wah = Screen.GetWorkingArea(this).Height;
            if (this.Height > wah) this.Height = wah;
            if (SuppressComment) this.Width = 415;

            Show();
            return true;
        }
    }
}
