using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI.AQUATOX
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form splashForm = new Splash();

            // Subscribe to its FormClosed event
            splashForm.FormClosed += new FormClosedEventHandler(OnFormClosed);

            // Show the splash form
            splashForm.Show();

            // Run the application
            Application.Run();
        }

        public static void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            ((Form)sender).FormClosed -= OnFormClosed;
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }

    }
}
