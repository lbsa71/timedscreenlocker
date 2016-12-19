namespace lbsa71.TimedScreenLocker.Core
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using System.Windows.Forms;

    using Timer = System.Timers.Timer;

    public abstract class TimedAppLauncher
    {
        private const long ticksInAYear = TimeSpan.TicksPerSecond * 60 * 60 * 24 * 365;

        protected string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        protected NotifyIcon NotifyIcon;

        private readonly string datestring = DateTime.Now.ToString("yyyyMd");

        private readonly string killFileName;

        private readonly Timer appCheckTimer;

        private long logInstance = DateTime.Now.Ticks % ticksInAYear;

        protected TimedAppLauncher()
        {
            this.appCheckTimer = new Timer(10000);

            this.appCheckTimer.Elapsed += this.CheckApp;

            this.CheckApp(null, null);

            this.killFileName = this.baseDirectory + "kill_me_" + this.datestring;
            this.Log("Killfile:[" + this.killFileName + "]");
        }

        protected abstract Icon AppIcon { get; }

        protected abstract string OtherProcessExeFileName { get; }

        public void Log(string message)
        {
            bool done = false;

            while (!done)
            {
                var logFile = this.baseDirectory + AppDomain.CurrentDomain.FriendlyName + "." + this.logInstance
                              + ".log";

                try
                {
                    using (var fs = new FileStream(logFile, FileMode.Append))
                    {
                        using (var logStream = new StreamWriter(fs) { AutoFlush = false })
                        {
                            logStream.WriteLine(DateTime.Now + ": " + message);
                            logStream.Flush();
                            logStream.Close();
                        }

                        fs.Close();
                        done = true;
                    }
                }
                catch (Exception)
                {
                    this.logInstance++;
                }
            }

            Console.WriteLine(message);
        }

        public virtual void Start()
        {
            this.NotifyIcon = new NotifyIcon { Icon = this.AppIcon, Text = "...Gäsp...", Visible = true };

            this.Log("AppTimer Start");
            this.appCheckTimer.Start();
        }

        public virtual void Stop()
        {
            this.Log("AppTimer Stop");
            this.appCheckTimer.Stop();

            this.NotifyIcon.Visible = false;
            this.NotifyIcon.Dispose();
            this.NotifyIcon = null;
        }

        protected virtual void SetText(string text)
        {
            var notifyIcon = this.NotifyIcon;
            if (notifyIcon != null)
            {
                this.Log("SetText: " + text);
                notifyIcon.Text = text;
            }
            else
            {
                this.Log("Failed to SetText: " + text);
            }
        }

        private void CheckApp(object sender, ElapsedEventArgs eventArgs)
        {
            try
            {
                var rescueText = AppDomain.CurrentDomain.FriendlyName + " till räddning!";

                if (File.Exists(this.killFileName) || Directory.Exists(this.killFileName))
                {
                    this.Log("Found killfile [" + this.killFileName + "]");

                    Application.Exit();
                }

                if (string.IsNullOrWhiteSpace(this.OtherProcessExeFileName))
                {
                    return;
                }

                var availableProcesses = this.GetAvailableProcesses();

                if (availableProcesses.Any(this.IsOtherProcess))
                {
                    this.Log("Found " + this.OtherProcessExeFileName);

                    if (this.GetText() == rescueText)
                    {
                        this.SetText("Zzzz...");
                    }
                }
                else
                {
                    this.Log("Did not find " + this.OtherProcessExeFileName);

                    var otherAppExe = this.baseDirectory + this.OtherProcessExeFileName;

                    this.Log("Starting  " + otherAppExe);

                    Process.Start(otherAppExe);

                    this.SetText(rescueText);
                }
            }
            catch (Exception e)
            {
                this.Log("CheckApp: " + e.Message);
            }
        }

        private Process[] GetAvailableProcesses()
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentProcessFilename = currentProcess.MainModule.FileName;
            var currentProcessId = currentProcess.Id;

            var otherProcesses = Process.GetProcesses();

            var availableprocesses = otherProcesses.Where(
                _ =>
                    {
                        try
                        {
                            var filename = Path.GetFileName(_.MainModule.FileName);
                            var id = _.Id;

                            if ((filename == currentProcessFilename) && (id != currentProcessId))
                            {
                                this.Log("Found sibling: " + filename + "[" + id + "]");
                                this.Log("I have: " + currentProcessFilename + "[" + currentProcessId + "]");
                                this.Log("Exiting. ");

                                Application.Exit();
                            }

                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }).ToArray();
            return availableprocesses;
        }

        private string GetText()
        {
            return this.NotifyIcon?.Text ?? "???";
        }

        private bool IsOtherProcess(Process unknown)
        {
            return Path.GetFileName(unknown.MainModule.FileName) == this.OtherProcessExeFileName;
        }
    }
}