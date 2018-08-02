using System;
using System.Windows.Forms;

namespace ChatClient
{
    internal static class ChatApplication
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatClientForm());
        }
    }
}
