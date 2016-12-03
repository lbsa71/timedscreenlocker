namespace lbsa71.TimedScreenLocker.TrayApp
{
    using System;
    using System.Drawing;

    using lbsa71.TimedScreenLocker.Core;
    using lbsa71.TimedScreenLocker.TrayApp.Properties;

    public class TrayEngine : CoreEngine
    {
        public override bool CheckOn
        {
            get
            {
                return !this.ScreenSaverRunning;
            }
        }

        protected override string OtherProcessExeFileName => "Tuck.exe";

        protected override Icon AppIcon =>Resources.appIcon;

        protected override void SwitchOff()
        {
            this.Log("Locking Screen.");

            LockWorkStation();
        }
    }
}