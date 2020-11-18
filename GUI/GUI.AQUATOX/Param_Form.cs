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
        ComboBox[] dropboxes;

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
                booledits[index].Size = new Size(390, 19);
                booledits[index].TabIndex = 0 + (2 * index);
                booledits[index].Text = Param.Name;
                new ToolTip().SetToolTip(booledits[index], Param.Name);
                booledits[index].Checked = ((TBoolParam)Param).Val;
                Controls.Add(booledits[index]);
                return;
            }

            labels[index] = new Label();
            if (Param is TSubheading)
            {
                labels[index].Text = ((TSubheading)Param).Val;
                labels[index].Location = new Point(12, top-2);
                labels[index].Size = new Size(360, 39);
                labels[index].Font = new Font(labels[index].Font.FontFamily, 12, FontStyle.Bold);
                labels[index].TextAlign = ContentAlignment.MiddleLeft;
                Controls.Add(labels[index]);
                return;
            }

            labels[index].Location = new Point(12, top);
            labels[index].Size = new Size(labelwidth, 39);
            labels[index].TextAlign = ContentAlignment.MiddleRight;
            
            if (SuppressSymbol||(Param is TStringParam)||(Param is TDropDownParam)) textstr = Param.Name;
                else textstr = Param.Symbol + " (" + Param.Name + ")";
            labels[index].Text = textstr;
            new ToolTip().SetToolTip(labels[index], textstr);
            Controls.Add(labels[index]);

            if (Param is TDropDownParam)
            {
                dropboxes[index] = new ComboBox();
                dropboxes[index].DropDownStyle = ComboBoxStyle.DropDown;
                if (((TDropDownParam) Param).ValList != null)
                 dropboxes[index].Items.AddRange(((TDropDownParam)Param).ValList);
                dropboxes[index].Location = new Point(labelwidth + 22, top + 9);
                dropboxes[index].Size = new Size(120, 23);
                dropboxes[index].TabIndex = 0 + (2 * index);
                dropboxes[index].Text = ((TDropDownParam)Param).Val;
                Controls.Add(dropboxes[index]);
                return;
            }


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
            edits[index].TabIndex = 0 + (2 * index);

            if (Param is TStringParam)
            {
                edits[index].Size = new Size(200, 23);
                edits[index].Text = ((TStringParam)Param).Val;
            }
            else
                    {
                        edits[index].Size = new Size(90, 23);
                        edits[index].Text = Param.Val.ToString();
                    }

            Controls.Add(edits[index]);

            units[index] = new Label();
            units[index].Location = new Point(labelwidth+115, top);
            units[index].Size = new Size(68, 39);
            units[index].Text = Param.Units;
            units[index].TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(units[index]);

            if (!(SuppressComment|| (Param is TStringParam) || (Param is TDropDownParam)))
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

        public bool ReadEdit(ref TParameter Param, int index)
        {
            double f;
            if (Param is TSubheading) return true;
            if (Param is TBoolParam) ((TBoolParam)Param).Val = booledits[index].Checked;
            else if (Param is TDateParam) ((TDateParam)Param).Val = dateedits[index].Value;
            else if (Param is TStringParam) ((TStringParam)Param).Val = edits[index].Text;
            else if (Param is TDropDownParam)
            { ((TDropDownParam)Param).Val = dropboxes[index].Text; }
            else if (double.TryParse(edits[index].Text, out f))
            {
                Param.Val = f;
                labels[index].BackColor = DefaultBackColor;
            }
            else
            {
                labels[index].BackColor = Color.Yellow;
                return false;
            };

            if (!SuppressComment)
             if (references[index]!=null)  // comments only for TParameter base object  
                Param.Comment = references[index].Text;
            return true;

        }


        private void Param_Form_FormClosed(object sender, FormClosedEventArgs e)
        {   
            
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

        private void Param_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UserCanceled) return;

            bool validinputs = true;
            for (int i = 0; i < nparam; i++)
            {
                validinputs = ReadEdit(ref plist[i], i) && validinputs;
            }

            if (!validinputs)
            {
                MessageBox.Show("Please fix invalid inputs (highlighted) or select cancel", "Information",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);
                e.Cancel = true;
            }
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
            dropboxes = new ComboBox[nparam];

            for (int i=0; i<nparam; i++)
            {
                AddEdit(ref parmlist[i], i);
            }

            // Adjust height and width of box to be appropriate for number of edits
            this.Height = Spacing * nparam + 75;
            int wah = Screen.GetWorkingArea(this).Height;
            if (this.Height > wah) this.Height = wah;
            if (SuppressComment) this.Width = 440;

            Show();
            return true;
        }
    }
}
