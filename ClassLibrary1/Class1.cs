﻿namespace ClassLibrary1
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using Timer = System.Threading.Timer;

    public abstract class Class1
    {
        public DateTime lastChecked;

        public bool ScreenSaverRunning;

        private double secondsOff;

        private double secondsOn;

        private readonly NotifyIcon notifyIcon1;

        private double secondsLeftOff;

        private double secondsLeftOn;

        private Timer timer1;

        private int pollingInterval;

        public Class1()
        {
            // Create the NotifyIcon.
            this.notifyIcon1 = new NotifyIcon();

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            this.notifyIcon1.Icon = new Icon("appicon.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            // notifyIcon1.ContextMenu = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            this.notifyIcon1.Text = "(Starting up)";
            this.notifyIcon1.Visible = true;

            SystemEvents.SessionSwitch += this.SystemEvents_SessionSwitch;

            this.lastChecked = DateTime.Now;

            this.secondsOff = double.Parse(ConfigurationManager.AppSettings["secondsOff"]) * 60;
            this.secondsOn = double.Parse(ConfigurationManager.AppSettings["secondsOn"]) * 60;

            this.pollingInterval = int.Parse(ConfigurationManager.AppSettings["pollingInterval"]);

            this.secondsLeftOff = this.secondsOff;
            this.secondsLeftOn = this.secondsOn;

            this.timer1 = new Timer(this.Check);
        }

        public abstract bool CheckOn { get; }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int fuWinIni);

        public void Check(object state)
        {
            var now = DateTime.Now;

            var secondssince = (now - this.lastChecked).TotalSeconds;
            this.lastChecked = now;

            if (this.CheckOn && (this.secondsLeftOn > 0))
            {
                this.secondsLeftOn -= secondssince;

                if (this.secondsLeftOn <= 0)
                {
                    this.secondsLeftOn = 0;
                    this.secondsLeftOff = this.secondsOff;
                }
            }
            else
            {
                if (!this.CheckOn && (this.secondsLeftOff > 0))
                {
                    this.secondsLeftOff -= secondssince;

                    if (this.secondsLeftOff <= 0)
                    {
                        this.secondsLeftOff = 0;
                        this.secondsLeftOn = this.secondsOn;
                    }
                }
            }

            var message = this.secondsLeftOn > 0
                              ? "Tid tills datorn låser:" + this.tillminut(this.secondsLeftOn)
                              : "Tid tills datorn är öppen igen:" + this.tillminut(this.secondsLeftOff);

            this.SetText(message);

            if (this.secondsLeftOn == 0) this.SwitchOff();
        }

        protected virtual void SetText(string text)
        {
            this.notifyIcon1.Text = text;
        }

        public void Start()
        {            
            this.timer1.Change(0, this.pollingInterval * 1000);
        }

        public void Stop()
        {
            this.timer1.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
        }

        protected abstract void SwitchOff();

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock) this.ScreenSaverRunning = true;
            else if (e.Reason == SessionSwitchReason.SessionUnlock) this.ScreenSaverRunning = false;
        }

        private string tillminut(double seconds)
        {
            var minuter = (int)(seconds / 60);
            var sekunder = (int)(seconds - minuter * 60);

            return string.Format("{0:D2}:{1:D2}", minuter, sekunder);
        }
    }
}