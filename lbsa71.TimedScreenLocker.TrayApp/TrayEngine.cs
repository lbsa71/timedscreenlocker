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

        protected override void SwitchOff()
        {
            this.Log("Locking Screen.");

            LockWorkStation();
        }
    }
}
