using Material3.WinForms.Theming;
using Material3.Core;

namespace ProjectRPL2_
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ThemeManager.Apply(MaterialTheme.FromSeed(new Argb(255, 76, 86, 220)), isDark: false);
            Application.Run(new FormLogin());
        }
    }
}

