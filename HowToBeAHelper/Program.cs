using System;
using System.Windows.Forms;

namespace HowToBeAHelper.Client
{
    internal static class Program
    {
        internal static MainForm Form { get; private set; }

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Form = new MainForm());
        }
    }
}
