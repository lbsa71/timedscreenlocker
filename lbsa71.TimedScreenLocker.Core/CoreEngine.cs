namespace lbsa71.TimedScreenLocker.Core
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Timers;

    using Microsoft.Win32;

    public abstract class CoreEngine : TimedAppLauncher
    {
        public bool ScreenSaverRunning;

        protected Timer screenCheckTimer;

        private readonly int pollingInterval;

        private readonly double secondsOff;

        private readonly double secondsOn;

        private DateTime lastChecked;

        private double secondsLeftOff;

        private double secondsLeftOn;

        private string TimeFile;

        protected CoreEngine()
        {
            SystemEvents.SessionSwitch += this.SystemEvents_SessionSwitch;

            this.lastChecked = DateTime.Now;

            this.secondsOff = double.Parse(ConfigurationManager.AppSettings["secondsOff"] ?? "40") * 60;
            this.secondsOn = double.Parse(ConfigurationManager.AppSettings["secondsOn"] ?? "45") * 60;

            this.pollingInterval = int.Parse(ConfigurationManager.AppSettings["pollingInterval"] ?? "20");

            this.TimeFile = this.baseDirectory + AppDomain.CurrentDomain.FriendlyName + ".time";

            this.LoadTime();

            this.screenCheckTimer = new Timer(this.pollingInterval * 1000);
            this.screenCheckTimer.Elapsed += this.Check;

            this.Log(
                "System initialized: secondsOff:" + this.secondsOff + " secondsOn:" + this.secondsOn
                + " pollingInterval:" + this.pollingInterval);
        }

        public abstract bool CheckOn { get; }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int fuWinIni);

        public override void Start()
        {
            this.Log("Timer Start");
            this.screenCheckTimer.Start();

            base.Start();
        }

        public override void Stop()
        {
            this.Log("Timer Stop");
            this.screenCheckTimer.Stop();

            base.Stop();
        }

        [DllImport("user32.dll")]
        protected static extern bool LockWorkStation();

        protected abstract void SwitchOff();

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        private void Check(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            var secondssince = (now - this.lastChecked).TotalSeconds;
            this.lastChecked = now;

            this.Log(now + ": Time since last check:" + secondssince);

            var checkOn = this.CheckOn;

            if (secondssince > (this.pollingInterval * 4))
            {
                this.Log("Lost conciousness for " + secondssince + " seconds.");

                // For some reason, we have been unconcious for a while; treat this as being off.
                this.DeductTimeOff(secondssince);
            }
            else
            {
                if (checkOn)
                {
                    if (this.secondsLeftOn > 0)
                    {
                        this.Log("Screen on; deducting " + secondssince + " seconds from screen on.");

                        this.secondsLeftOn -= secondssince;

                        if (this.secondsLeftOn <= 0)
                        {
                            this.secondsLeftOn = 0;
                            this.secondsLeftOff = this.secondsOff;
                        }
                    }
                }
                else
                {
                    this.DeductTimeOff(secondssince);
                }
            }

            var message = this.secondsLeftOn > 0
                              ? "Tid tills datorn låser:" + this.tillminut(this.secondsLeftOn)
                              : "Tid tills datorn är öppen igen:" + this.tillminut(this.secondsLeftOff);

            this.SetText(message);

            if (this.secondsLeftOn <= 0)
            {
                this.SwitchOff();
            }

            this.SaveTime();
        }

        private void DeductTimeOff(double secondssince)
        {
            this.Log("Screen off; deducting " + secondssince + " seconds from secondsoff.");

            this.secondsLeftOff -= secondssince;

            if (this.secondsLeftOff <= 0)
            {
                this.secondsLeftOff = 0;
                this.secondsLeftOn = this.secondsOn;
            }
        }

        private void LoadTime()
        {
            this.Log("LoadTime");

            var timeFile = this.TimeFile;

            if (timeFile != null)
            {
                try
                {
                    using (var fs = new FileStream(timeFile, FileMode.OpenOrCreate))
                    {
                        using (var logStream = new StreamReader(fs))
                        {
                            var secondsLeftOnString = logStream.ReadLine();

                            if (string.IsNullOrWhiteSpace(secondsLeftOnString))
                            {
                                this.secondsLeftOn = 0;                              
                            }
                            else
                            {
                                this.secondsLeftOn = double.Parse(secondsLeftOnString);
                            }

                            var secondsLeftOffString = logStream.ReadLine();

                            if (string.IsNullOrWhiteSpace(secondsLeftOffString))
                            {
                                this.secondsLeftOff = this.secondsOff;
                            }
                            else
                            {
                                this.secondsLeftOff = double.Parse(secondsLeftOffString);
                            }

                            logStream.Close();
                        }

                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    this.Log(e.Message);

                    this.secondsLeftOn = 0;
                    this.secondsLeftOff = this.secondsOff;

                    this.Log("Disabling Time File");

                    this.TimeFile = null;
                }
            }
        }

        private void SaveTime()
        {
            this.Log("SaveTime:" + this.secondsLeftOn + ", " + this.secondsLeftOff);

            var timeFile = this.TimeFile;

            if (timeFile != null)
            {
                try
                {
                    using (var fs = new FileStream(timeFile, FileMode.Create))
                    {
                        using (var logStream = new StreamWriter(fs) { AutoFlush = false })
                        {
                            logStream.WriteLine(this.secondsLeftOn);
                            logStream.WriteLine(this.secondsLeftOff);
                            logStream.Flush();
                            logStream.Close();
                        }

                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    // -- something seems to be problematic with the file system. Disable time storing.
                    this.Log(e.Message);

                    this.Log("Disabling Time File");
                    this.TimeFile = null;
                }
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            this.Log("SystemEvents_SessionSwitch:" + e.Reason);

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                this.ScreenSaverRunning = true;
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                this.ScreenSaverRunning = false;
            }
        }

        private string tillminut(double seconds)
        {
            var minuter = (int)(seconds / 60);
            var sekunder = (int)(seconds - minuter * 60);

            return string.Format("{0:D2}:{1:D2}", minuter, sekunder);
        }
    }
}