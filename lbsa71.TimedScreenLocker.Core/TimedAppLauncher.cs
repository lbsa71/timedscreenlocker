namespace lbsa71.TimedScreenLocker.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Timers;

    public abstract class TimedAppLauncher
    {
        protected string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private Timer appCheckTimer;

        protected TimedAppLauncher()
        {
            this.Log(this.baseDirectory);

            this.appCheckTimer = new Timer(3000);
            this.appCheckTimer.Elapsed += this.CheckApp;
        }

        protected abstract string otherProcessExeFileName { get; }

        public void Log(string message)
        {
            var logFile = this.baseDirectory + AppDomain.CurrentDomain.FriendlyName + ".log";

            using (var fs = new FileStream(logFile, FileMode.Append))
            {
                using (var logStream = new StreamWriter(fs) { AutoFlush = false })
                {
                    logStream.WriteLine(message);
                    logStream.Flush();
                    logStream.Close();
                }

                fs.Close();
            }

            Console.WriteLine(message);
        }

        public virtual void Start()
        {
            this.Log("AppTimer Start");
            this.appCheckTimer.Start();
        }

        public virtual void Stop()
        {
            this.Log("AppTimer Stop");
            this.appCheckTimer.Stop();
        }

        private void CheckApp(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.otherProcessExeFileName))
            {
                return;
            }

            var otherProcesses = Process.GetProcesses();

            if (otherProcesses.Any(
                _ =>
                    {
                        try
                        {
                            var fileName = Path.GetFileName(_.MainModule.FileName);
                            if (fileName == this.otherProcessExeFileName)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }))
            {
                this.Log("Found " + this.otherProcessExeFileName);
            }
            else
            {
                this.Log("Did not find " + this.otherProcessExeFileName);

                var otherAppExe = this.baseDirectory + this.otherProcessExeFileName;

                Process.Start(otherAppExe);
            }
        }
    }
}