using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Snake_SFML.NET
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Game game = new Game();

            Application.Run(game.settingsMenu);
        }
    }
}