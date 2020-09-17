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
        int Spacing = 36;
        public bool SuppressSymbol=false, SuppressComment=false;
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

            int top = 16 + (Spacing * index);

            if (Param is TBoolParam)
            {
                booledits[index] = new CheckBox();
                booledits[index].FlatAppearance.BorderSize = 2;
                booledits[index].Location = new Point(30,top+10);
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
                labels[index].Font = new Font(labels[index].Font.FontFamily, 14);
                labels[index].TextAlign = ContentAlignment.MiddleLeft;
                Controls.Add(labels[index]);
                return;
            }

            labels[index].Location = new Point(12, top);
            labels[index].Size = new Size(145, 39);
            labels[index].TextAlign = ContentAlignment.MiddleRight;
            if (SuppressSymbol) textstr = Param.Name;
                else textstr = Param.Symbol + " (" + Param.Name + ")";
            labels[index].Text = textstr;
            Controls.Add(labels[index]);
            
            if (Param is TDateParam)
            {
                dateedits[index] = new DateTimePicker();
                dateedits[index].Location = new Point(163, top + 9);
                dateedits[index].Size = new Size(200, 23);
                dateedits[index].TabIndex = 0 + (2 * index);
                dateedits[index].Value = ((TDateParam)Param).Val;
                Controls.Add(dateedits[index]);
                return;
            }

            edits[index] = new TextBox();
            edits[index].Location = new Point(163, top + 9);
            edits[index].Size = new Size(100, 23);
            edits[index].TabIndex = 0 + (2 * index);
            edits[index].Text = Param.Val.ToString();
            Controls.Add(edits[index]);

            units[index] = new Label();
            units[index].Location = new Point(269, top);
            units[index].Size = new Size(68, 39);
            units[index].Text = Param.Units;
            units[index].TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(units[index]);

            if (!SuppressComment)
                {
                    references[index] = new TextBox();
                    references[index].Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)| AnchorStyles.Right)));
                    references[index].Location = new Point(352, top + 9);
                    references[index].TabIndex = 1 + (2 * index);
                    references[index].Size = new Size(564, 23);
                    references[index].Text = Param.Comment;
                    Controls.Add(references[index]);
                 }

        }

        public bool EditParams(ref TParameter[] parmlist)
        {
            int nparam = parmlist.Length;
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
            if (SuppressComment) this.Width = 420;

            Show();
            return true;
        }
    }
}
