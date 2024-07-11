using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class MergeForm : Form
    {
        public double Threshold { get; private set; }
        List<double> TravelTimes;
        public MergeForm((int segmentCount, double averageTravelTime, double minTravelTime, double fifthPctileTravelTime, List<double> TTimes) ttdata)
        {
            InitializeComponent();
            SILabel2.Text = $"The current network has {ttdata.segmentCount} segments. " +
                            $"The average travel time is {ttdata.averageTravelTime:F3} days with a minimum of {ttdata.minTravelTime:F3} days. " +
                            $"The 5th percentile retention time is {ttdata.fifthPctileTravelTime:F3} days.";
            TravelTimes = ttdata.TTimes;
            Travel_Time_Edit_TextChanged(null, null);
        }

        private void Travel_Time_Edit_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(Travel_Time_Edit.Text, out double enteredValue))
            {
                int count = TravelTimes.Count(time => time < enteredValue);
                label2.Text = $"days ({count} segments)";
            }
            // If the text is not a valid number, do nothing
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(Travel_Time_Edit.Text, out double parsedValue))
            {
                Threshold = parsedValue;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Keep the dialog open by not closing it
            }
        }

        private void HelpButton2_Click(object sender, EventArgs e)
        {
            string target = "MergeSegments";
            AQTMainForm.OpenUrl(target);
        }
    }

}
