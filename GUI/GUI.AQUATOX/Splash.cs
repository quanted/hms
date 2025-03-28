using AQUATOX.AQSim_2D;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static AQUATOX.AQSim_2D.AQSim_2D;



// Menu View/Terminal  "dotnet publish hms.sln -c Release -r win-x64 --self-contained true"


namespace GUI.AQUATOX
{
    public partial class Splash : Form
    {
        private Point mouseOffset;

        public static void ScaleFonts(Form frm, Control parent)
            // This procedure is necessary if non-default fonts are used on a form and "make text bigger" is selected within Windows Setup options 
            // non-default fonts will not scale by default thus causing form rendering problems.
        {
            float systemDefaultSize = SystemFonts.DefaultFont.Size;  
            float currentFormSize = frm.Font.Size;

            // If scaling is ~100%, skip font adjustment entirely
            if ((currentFormSize<10) || (Math.Abs(currentFormSize - systemDefaultSize) < 0.1f))
                return;

            float scaleFactor = (currentFormSize / systemDefaultSize) * 0.85f;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Font == frm.Font)
                {
                    if (ctrl.HasChildren)
                        ScaleFonts(frm, ctrl);
                    continue;
                }

                Font oldFont = ctrl.Font;
                float newSize = oldFont.Size * scaleFactor;

                // Only update if size would actually change
                if (Math.Abs(oldFont.Size - newSize) > 0.1f)
                {
                    ctrl.Font = new Font(oldFont.FontFamily, newSize, oldFont.Style);
                }

                if (ctrl.HasChildren)
                    ScaleFonts(frm, ctrl);
            }
        }

        public Splash()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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

                // Check if the JSON file exists
                if (File.Exists("..\\2D_Inputs\\webServiceURLs.JSON"))
                    try
                    {
                        string jsonContent = File.ReadAllText("..\\2D_Inputs\\webServiceURLs.JSON");
                        AQSim_2D.webServiceURLs = JsonConvert.DeserializeObject<webServiceURLsClass>(jsonContent);
                        if (webServiceURLs == null) AQSim_2D.webServiceURLs = new();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading or deserializing JSON file 2D_Inputs\\webServiceURLs.JSON: " + ex.Message);
                        AQSim_2D.webServiceURLs = new();
                    }

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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Capture the mouse position relative to the form
            mouseOffset = new Point(e.Location.X, e.Location.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            // Check if the left mouse button is pressed (dragging)
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the new form location based on the mouse movement
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-mouseOffset.X, -mouseOffset.Y);

                // Set the new location for the form
                this.Location = newLocation;
            }
        }

        private void Splash_Shown(object sender, EventArgs e)
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(exeDirectory);
            AQTMainForm.defaultBrowser = Properties.Settings.Default.BrowserExe;
        }

        private void Help_Button_Click(object sender, EventArgs e)
        {
            string target = "splash";
            AQTMainForm.OpenUrl(target);
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            ScaleFonts(this,this);
        }
    }
}
