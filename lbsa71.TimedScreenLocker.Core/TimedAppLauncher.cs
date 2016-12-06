namespace lbsa71.TimedScreenLocker.Core
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Timers;
    using System.Windows.Forms;

    using Timer = System.Timers.Timer;

    public abstract class TimedAppLauncher
    {
        private const long ticksInAYear = TimeSpan.TicksPerSecond * 60 * 60 * 24 *365;

        private long logInstance = DateTime.Now.Ticks % ticksInAYear;

        protected NotifyIcon NotifyIcon;
        protected abstract Icon AppIcon { get; }

        protected string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private Timer appCheckTimer;

        protected TimedAppLauncher()
        {          
            this.appCheckTimer = new Timer(10000);

            this.appCheckTimer.Elapsed += this.CheckApp;
        }

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
                    this.logInstance ++;
                }
            }

            Console.WriteLine(message);
        }

        public virtual void Start()
        {
            this.NotifyIcon = new NotifyIcon
            {
                Icon = this.AppIcon,
                Text = "...Gäsp...",
                Visible = true
            };

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

        private void CheckApp(object sender, ElapsedEventArgs eventArgs)
        {
            try
            {
                var rescueText = AppDomain.CurrentDomain.FriendlyName + " till räddning!";

                var killfile = this.baseDirectory + "killme2";

                if (File.Exists(killfile) || Directory.Exists(killfile))
                {
                    Application.Exit();
                }

                if (string.IsNullOrWhiteSpace(this.OtherProcessExeFileName))
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
                            if (fileName == this.OtherProcessExeFileName)
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
                    this.Log("Found " + this.OtherProcessExeFileName);

                    if (this.GetText() == rescueText)
                    {
                        SetText("Zzzz...");
                    }
                }
                else
                {
                    this.Log("Did not find " + this.OtherProcessExeFileName);

                    var otherAppExe = this.baseDirectory + this.OtherProcessExeFileName;

                    Process.Start(otherAppExe);

                    SetText(rescueText);
                }
            }
            catch (Exception e)
            {
                this.Log("CheckApp: " + e.Message);                
            }                      
        }

        private string GetText()
        {
            return this.NotifyIcon?.Text ?? "???";
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
    }
}