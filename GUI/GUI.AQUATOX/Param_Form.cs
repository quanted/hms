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
        bool suppressing = false;

        public bool SuppressSymbol = false, SuppressComment = false;
        bool UserCanceled = false;
        int nparam;
        int nRendered;
        TParameter[] plist;

        Button[] buttons;
        TextBox[] edits;
        Label[] labels;
        Label[] units;
        TextBox[] references;
        DateTimePicker[] dateedits;
        CheckBox[] booledits;
        ComboBox[] dropboxes;


        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that 
        /// window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Suspends painting for the target control. Do NOT forget to call EndControlUpdate!!!
        /// </summary>
        /// <param name="control">visual control</param>
        public static void BeginControlUpdate(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Resumes painting for the target control. Intended to be called following a call to BeginControlUpdate()
        /// </summary>
        /// <param name="control">visual control</param>
        public static void EndControlUpdate(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
        }

        public Param_Form()
        {
            AutoScroll = true;
            InitializeComponent();
        }

        public void AdjustEdit(ref TParameter Param, int index)
        {

            int AY = this.AutoScrollPosition.Y;
            int AX = this.AutoScrollPosition.X;  // required to place control in correct location and account for scroll-bar behavior.

            if ((!suppressing) || (Param is TSubheading)) nRendered++;  // always render sub-headings
            int top = topbuffer + (Spacing * nRendered);

            if (Param is TBoolParam)
            {
                booledits[index].Location = new Point(AX + 30, top + 12 + AY);
                booledits[index].Visible = !suppressing;
                return;
            }

            if (Param is TSubheading)
            {
                labels[index].Location = new Point(AX +12, top - 2 + AY);

                if (index > 0)
                {
                    suppressing = !(((TSubheading)Param).expanded);
                    int left = 330;
                    if (Spacing == 28) left = 330;
                    buttons[index].Location = new Point(AX +left, top + 8 + AY);

                    if (!SuppressComment)  //no subheading info on narrow parameter forms
                        units[index].Location = new Point(AX +395, top - 1 + AY);
                }
                return;
            }

            labels[index].Location = new Point(AX +12, top + AY);
            labels[index].Visible = !suppressing;

            if (Param is TDropDownParam)
            {
                dropboxes[index].Location = new Point(AX +labelwidth + 22, top + 9 + AY);
                dropboxes[index].Visible = !suppressing;
                return;
            }


            if (Param is TDateParam)
            {
                dateedits[index].Location = new Point(AX +labelwidth + 22, top + 9 + AY);
                dateedits[index].Visible = !suppressing;
                return;
            }

            edits[index].Location = new Point(AX +labelwidth + 22, top + 9 + AY);
            edits[index].Visible = !suppressing;

            units[index].Location = new Point(AX +labelwidth + 115, top + AY);
            units[index].Visible = !suppressing;

            if (!(SuppressComment || (Param is TStringParam) || (Param is TDropDownParam)))
            {
                references[index].Location = new Point(labelwidth + 190, top + 9 + AY);
                references[index].Visible = !suppressing;
            }
        }

        public void AddEdit(ref TParameter Param, int index)
        {
            string textstr;

            if ((!suppressing)|| (Param is TSubheading)) nRendered++;  // always render sub-headings
            int top = topbuffer + (Spacing * nRendered);

            if (Param is TBoolParam)
            {
                booledits[index] = new CheckBox();
                booledits[index].FlatAppearance.BorderSize = 2;
                booledits[index].Location = new Point(30, top + 12);
                booledits[index].Size = new Size(390, 19);
                booledits[index].TabIndex = 0 + (2 * index);
                booledits[index].Text = Param.Name;
                new ToolTip().SetToolTip(booledits[index], Param.Name);
                booledits[index].Checked = ((TBoolParam)Param).Val;
                booledits[index].Visible = !suppressing;
                Controls.Add(booledits[index]);
                return;
            }

            labels[index] = new Label();
            if (Param is TSubheading)
            {
                labels[index].Text = ((TSubheading)Param).Val;
                labels[index].Location = new Point(12, top - 2);
                labels[index].Size = new Size(360, 39);
                labels[index].Font = new Font(labels[index].Font.FontFamily, 12, FontStyle.Bold);
                labels[index].TextAlign = ContentAlignment.MiddleLeft;

                if (index > 0)
                {
                    suppressing = !(((TSubheading)Param).expanded);
                    buttons[index] = new Button();
                    if (((TSubheading)Param).expanded) buttons[index].Text = "Collapse";
                    else buttons[index].Text = "Expand";

                    int left = 330;
                    if (Spacing == 28) left = 330;
                    buttons[index].Location = new Point(left, top + 8);
                    buttons[index].Font = new Font(buttons[index].Font.FontFamily, 8);

                    buttons[index].Size = new Size(60, 20);
                    buttons[index].Click += new EventHandler(NewButton_Click);
                    buttons[index].Tag = index;

                    Controls.Add(buttons[index]);

                    if (!SuppressComment)  //no subheading info on narrow parameter forms
                    {
                        units[index] = new Label();  // Borrow "units" labels to add information about the subheading
                        units[index].Location = new Point(395, top - 1);
                        units[index].Size = new Size(500, 39);
                        units[index].Text = Param.Comment;
                        units[index].Font = new Font(labels[index].Font.FontFamily, 9, FontStyle.Italic);
                        units[index].TextAlign = ContentAlignment.MiddleLeft;
                        Controls.Add(units[index]);
                    }
                }

                Controls.Add(labels[index]);
                return;
            }

            labels[index].Location = new Point(12, top);
            labels[index].Size = new Size(labelwidth, 39);
            labels[index].TextAlign = ContentAlignment.MiddleRight;

            if (SuppressSymbol || (Param is TStringParam) || (Param is TDropDownParam)) textstr = Param.Name;
            else textstr = Param.Symbol + " (" + Param.Name + ")";
            labels[index].Text = textstr;
            new ToolTip().SetToolTip(labels[index], textstr);
            labels[index].Visible = !suppressing;
            Controls.Add(labels[index]);


            if (Param is TDropDownParam)
            {
                dropboxes[index] = new ComboBox();
                dropboxes[index].DropDownStyle = ComboBoxStyle.DropDown;
                if (((TDropDownParam)Param).ValList != null)
                    dropboxes[index].Items.AddRange(((TDropDownParam)Param).ValList);
                dropboxes[index].Location = new Point(labelwidth + 22, top + 9);
                dropboxes[index].Size = new Size(120, 23);
                dropboxes[index].TabIndex = 0 + (2 * index);
                dropboxes[index].Text = ((TDropDownParam)Param).Val;
                dropboxes[index].Visible = !suppressing;
                Controls.Add(dropboxes[index]);
                return;
            }


            if (Param is TDateParam)
            {
                dateedits[index] = new DateTimePicker();
                dateedits[index].CustomFormat = "MM/dd/yyyy";
                dateedits[index].Format = DateTimePickerFormat.Custom;
                dateedits[index].Location = new Point(labelwidth + 22, top + 9);
                dateedits[index].Size = new Size(95, 23);
                dateedits[index].TabIndex = 0 + (2 * index);
                dateedits[index].Value = ((TDateParam)Param).Val;
                dateedits[index].Visible = !suppressing;
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
            edits[index].Visible = !suppressing;
            Controls.Add(edits[index]);

            units[index] = new Label();
            units[index].Location = new Point(labelwidth + 115, top);
            units[index].Size = new Size(68, 39);
            units[index].Text = Param.Units;
            units[index].TextAlign = ContentAlignment.MiddleLeft;
            units[index].Visible = !suppressing;
            Controls.Add(units[index]);

            if (!(SuppressComment || (Param is TStringParam) || (Param is TDropDownParam)))
            {
                references[index] = new TextBox();
                references[index].Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
                references[index].Location = new Point(labelwidth + 190, top + 9);
                references[index].TabIndex = 1 + (2 * index);
                references[index].Size = new Size(720 - labelwidth, 23);
                references[index].Text = Param.Comment;
                references[index].Visible = !suppressing;
                Controls.Add(references[index]);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Text == "Collapse") btn.Text = "Expand";
            else btn.Text = "Collapse";

            int i = (int)btn.Tag;
            ((TSubheading)plist[i]).expanded = !((TSubheading)plist[i]).expanded;

            AdjustVisibility();

        }

        TSubheading RelatedSubheading;
        public bool ReadEdit(ref TParameter Param, int index, bool copyvalue)
        {
            double f;

            if (Param is TSubheading)
            {
                if (index > 0) suppressing = !(((TSubheading)Param).expanded);
                RelatedSubheading = (TSubheading)Param;
                return true;
            }

            if (Param is TBoolParam) { if (copyvalue) ((TBoolParam)Param).Val = booledits[index].Checked; }
            else if (Param is TDateParam) { if (copyvalue) ((TDateParam)Param).Val = dateedits[index].Value; }
            else if (Param is TStringParam) { if (copyvalue) ((TStringParam)Param).Val = edits[index].Text; }
            else if (Param is TDropDownParam) { if (copyvalue) ((TDropDownParam)Param).Val = dropboxes[index].Text; }

            else if (double.TryParse(edits[index].Text, out f))  // TParameter base object with numercal input
            {
                Param.Val = f;
                if (copyvalue) labels[index].BackColor = DefaultBackColor;
            }
            else
            {
                labels[index].BackColor = Color.Yellow;
                if (suppressing) { RelatedSubheading.expanded = true; }
                return false;
            };

            if (!SuppressComment)
                if (references[index] != null)  // comments only for TParameter base object  
                    if (copyvalue) Param.Comment = references[index].Text;

            return true;

        }


        private void Param_Form_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void cancel_click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel any changes to inputs?", "Confirm",
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

        private bool ValidInputs()
        {
            bool validinputs = true;
            suppressing = false;

            for (int i = 0; i < nparam; i++)
            {
              validinputs = ReadEdit(ref plist[i], i, false) && validinputs;
            }
            return validinputs;
        }

        private void ReadInputs()
        {
            suppressing = false;

            for (int i = 0; i < nparam; i++)
            {
               ReadEdit(ref plist[i], i, true);
            }
        }

        private void Param_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UserCanceled) return;

            if (!ValidInputs())
            {
                AdjustVisibility();
                MessageBox.Show("Please fix invalid inputs (highlighted) or select cancel", "Information",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);
                e.Cancel = true;
            }

            ReadInputs();
        }


        public void CreateControls()
        {
            BeginControlUpdate(this);
            suppressing = false;
            nRendered = 0;

            for (int i = 0; i < nparam; i++)
            {
                AddEdit(ref plist[i], i);
            }

            ResizeScreen();
            EndControlUpdate(this);
        }

        public void ResizeScreen()
        {
            // Adjust height and width of box to be appropriate for number of edits
            this.Height = Spacing * nRendered + 100;
            int wah = Screen.GetWorkingArea(this).Height;
            if (this.Height > wah) this.Height = wah;
            if (SuppressComment) this.Width = 440;

        }

        public void AdjustVisibility()
        {
            BeginControlUpdate(this);
            suppressing = false;
            nRendered = 0;

            for (int i = 0; i < nparam; i++)
                {
                    AdjustEdit(ref plist[i], i);
                }

            ResizeScreen();
            EndControlUpdate(this);
        }

        public void removeControls(int i)
        {
            if (edits[i] != null)  Controls.Remove(edits[i]);
            if (labels[i] != null)  Controls.Remove(labels[i]);
            if (units[i] != null)  Controls.Remove(units[i]);
            if (references[i] != null)  Controls.Remove(references[i]);
            if (dateedits[i] != null)  Controls.Remove(dateedits[i]);
            if (booledits[i] != null)  Controls.Remove(booledits[i]);
            if (dropboxes[i] != null)  Controls.Remove(dropboxes[i]);
            if (buttons[i] != null) Controls.Remove(buttons[i]);
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
            buttons = new Button[nparam];
            references = new TextBox[nparam];
            dateedits = new DateTimePicker[nparam];
            booledits = new CheckBox[nparam];
            dropboxes = new ComboBox[nparam];

            CreateControls();

            ShowDialog();
            return true;
        }
    }
}
