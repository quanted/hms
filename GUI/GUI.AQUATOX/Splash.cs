﻿using AQUATOX.AQSim_2D;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AQUATOX.AQSim_2D.AQSim_2D;


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
    }
}