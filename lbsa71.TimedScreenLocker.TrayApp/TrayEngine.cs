namespace lbsa71.TimedScreenLocker.TrayApp
{
    using lbsa71.TimedScreenLocker.Core;

    public class TrayEngine : CoreEngine
    {
        public override bool CheckOn
        {
            get
            {
                return !this.ScreenSaverRunning;
            }
        }

        protected override string otherProcessExeFileName => "Tuck.exe";

        protected override void SwitchOff()
        {
            this.Log("Locking Screen.");

            LockWorkStation();
        }
    }
}
