using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lbsa71.TimedScreenLocker.Tuck
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var tuck = new TuckEngine();
            tuck.Start();

            try
            {
                Application.Run();
            }
            catch (Exception e)
            {
                tuck.Log("Main Exception: " + e.Message);
            }

            tuck.Stop();
        }
    }
}
