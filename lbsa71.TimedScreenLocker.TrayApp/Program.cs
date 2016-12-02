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
            notificationApp.Log(DateTime.Now + ": Starting up TrayApp.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var notificationApp = new TrayEngine();
            notificationApp.Start();

            Application.Run();

            notificationApp.Stop();

            notificationApp.Log(DateTime.Now + ": Exiting app.");
        }
    }
}