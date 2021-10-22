using AQUATOX.AQTSegment;
using Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace GUI.AQUATOX
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }


    public partial class GraphSetupForm : Form
    {

        public AQUATOXSegment outSeg = null;
        public TGraphSetup Graph;
        private bool UserCanceled = false;

        public GraphSetupForm()
        {
            InitializeComponent();
            this.PerformLayout();
        }

        public bool NameInList(string st)
        {
            foreach(TGraphSetup TG in outSeg.Graphs.GList) 
            {
                if (TG.GraphName == st) return true;
            }
            return false;
        }

        public bool EditGraph(AQUATOXSegment oS, int gIndx)
        {
            outSeg = oS;
            if (gIndx > -1) Graph = outSeg.Graphs.GList[gIndx];
            
            else
            {
                int iter = 1;
                string graphname = "Graph 1";
                while (NameInList(graphname)) { graphname = "Graph " + (iter++); }
                Graph = new TGraphSetup(graphname);
            }

            listBox2.Items.Clear();
            textBox1.Text = Graph.GraphName;
            foreach (SeriesID ser in Graph.YItems)
            {
                listBox2.Items.Add(ser.nm);
            }

            UpdateListBoxes();

            SetButtonsEditable();

            UserCanceled = false;
            ShowDialog();

            if (!UserCanceled) 
            {
                Graph.GraphName = textBox1.Text;
                Graph.YItems.Clear();

                foreach (TStateVariable TSV in outSeg.SV) 
                  if (TSV.SVoutput != null)
                    {
                        List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                        int ccount = vallist.Count();
                        for (int col = 1; col <= ccount; col++)
                        {
                            string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
                                 TSV.SVoutput.Metadata["Name_" + col.ToString()] +
                                 " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";

                            if (listBox2.Items.Contains(sertxt))
                                Graph.YItems.Add(new SeriesID() { nm = sertxt, lyr = TSV.Layer, ns=TSV.NState, typ=TSV.SVType, indx=col});

                        }
                    }

                if (gIndx < 0) outSeg.Graphs.GList.Add(Graph);
            }
            return !UserCanceled;
        }


        private void UpdateListBoxes()
        {
            listBox1.Items.Clear();
            foreach (TStateVariable TSV in outSeg.SV) 
              if ((TSV.SVoutput != null) && (TSV.IsPlant() || !PlantBox.Checked) && (TSV.IsAnimal() || !AnimalBox.Checked))
                {
                    List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                    int ccount = vallist.Count();
                    if (!ShowRatesBox.Checked) ccount = 1;
                    for (int col = 1; col <= ccount; col++)
                    {
                        string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
                             TSV.SVoutput.Metadata["Name_" + col.ToString()] +
                             " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";

                        if ((string.IsNullOrEmpty(SubstringBox.Text))||(sertxt.Contains(SubstringBox.Text, StringComparison.OrdinalIgnoreCase)))
                         {

                            if (!listBox2.Items.Contains(sertxt))
                                listBox1.Items.Add(sertxt);
                        }
                    }
                }

        }

        // Move selected items from one ListBox to another.
        private void MoveSelected(ListBox lstFrom, ListBox lstTo)
        {
            while (lstFrom.SelectedItems.Count > 0)
            {
                string item = (string)lstFrom.SelectedItems[0];
                lstTo.Items.Add(item);
                lstFrom.Items.Remove(item);
            }
            SetButtonsEditable();
        }

        // Enable and disable buttons.
        private void SetButtonsEditable()
        {
            btnSelect.Enabled = (listBox1.SelectedItems.Count > 0);
            btnSelectAll.Enabled = (listBox1.Items.Count > 0);
            btnDeselect.Enabled = (listBox2.SelectedItems.Count > 0);
            btnDeselectAll.Enabled = (listBox2.Items.Count > 0);
        }

        // Move all items from one ListBox to another.
        private void MoveAllItems(ListBox lstFrom, ListBox lstTo)
        {
            lstTo.Items.AddRange(lstFrom.Items);
            lstFrom.Items.Clear();
            SetButtonsEditable();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MoveSelected(listBox1, listBox2);
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBox1, listBox2);
        }

        private void includeRatesBox_Changed(object sender, EventArgs e)
        {
            UpdateListBoxes();
        }

        private void listBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            SetButtonsEditable();
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBox2, listBox1);
        }

        private void btnDeselect_Click_1(object sender, EventArgs e)
        {
            MoveSelected(listBox2, listBox1);
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel any changes to this graph?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                UserCanceled = true;
                this.Close();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
