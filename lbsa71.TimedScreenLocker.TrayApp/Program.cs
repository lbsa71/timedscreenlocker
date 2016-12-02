namespace lbsa71.TimedScreenLocker.TrayApp
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var notificationApp = new TrayEngine();

            notificationApp.Log(DateTime.Now + ": Starting up TrayApp.");

            notificationApp.Start();

            Application.Run();

            notificationApp.Stop();

            notificationApp.Log(DateTime.Now + ": Exiting app.");
        }
    }
}