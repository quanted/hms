using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class JSONEditForm : Form
    {
        public string jsonString;

        public JSONEditForm(string json)
        {
            InitializeComponent();
            jsonString = json;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            CancelButton = cancelButton;
            textBox.Text = jsonString;

            StartPosition = FormStartPosition.CenterScreen;
            Width = 600;
            Height = 400;
        }

        public string GetEditedJson()
        {
            return textBox.Text;
        }
    }

}
