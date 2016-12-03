namespace lbsa71.TimedScreenLocker.Test
{
    using System;
    using System.Drawing;

    using lbsa71.TimedScreenLocker.Core;

    internal class TestEngine : CoreEngine
    {
        public override bool CheckOn
        {
            get
            {
                var screenOn = !this.ScreenSaverRunning;

                this.Log("State on:" + screenOn);

                return screenOn;
            }
        }

        protected override Icon AppIcon => Resource.appIcon;

        protected override string OtherProcessExeFileName => null;

        protected override void SetText(string text)
        {
            this.Log(text);
        }

        protected override void SwitchOff()
        {
            this.Log("Locking Screen.");

            LockWorkStation();
        }
    }
}