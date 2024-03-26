using System.Windows.Forms;

namespace GUI.AQUATOX
{
    public partial class JSONEditForm : Form
    {
        public string jsonString;
        public string apiString;

        public JSONEditForm(string json, string apikey)
        {
            InitializeComponent();
            jsonString = json;
            apiString = apikey;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            CancelButton = cancelButton;
            textBox.Text = jsonString;
            API_key_textbox.Text = apiString;

            StartPosition = FormStartPosition.CenterScreen;
            Width = 745;
            Height = 545;
        }

        public (string,string) GetEditedJson()
        {
            return (API_key_textbox.Text, textBox.Text);
        }
    }

}
