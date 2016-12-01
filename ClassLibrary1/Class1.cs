namespace ClassLibrary1
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Timers;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using Timer = System.Timers.Timer;

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

            var appiconIco = "appicon.ico";

            var appIconPath = AppDomain.CurrentDomain.BaseDirectory + appiconIco;

            Console.WriteLine(appIconPath);

            this.notifyIcon1.Icon = new Icon(appIconPath);

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

            this.timer1 = new Timer(this.pollingInterval * 1000);
            this.timer1.Elapsed += this.Check;

            Console.WriteLine("System initialized: secondsOff:" + this.secondsOff + " secondsOn:" + this.secondsOn + " pollingInterval:" + this.pollingInterval);
        }

        private void Check(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            var secondssince = (now - this.lastChecked).TotalSeconds;
            this.lastChecked = now;

            Console.WriteLine(now + ": Time since last check:" + secondssince);

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

        public abstract bool CheckOn { get; }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int fuWinIni);

        protected virtual void SetText(string text)
        {
            Console.WriteLine("SetText: " + text);

            this.notifyIcon1.Text = text;
        }

        public void Start()
        {
            Console.WriteLine("Timer Start");
            this.timer1.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Timer Stop");
            this.timer1.Stop();
        }

        protected abstract void SwitchOff();

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Console.WriteLine("SystemEvents_SessionSwitch:" + e.Reason);

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