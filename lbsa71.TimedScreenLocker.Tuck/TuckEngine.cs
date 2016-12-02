namespace lbsa71.TimedScreenLocker.Tuck
{
    using lbsa71.TimedScreenLocker.Core;

    public class TuckEngine : TimedAppLauncher
    {
        protected override string otherProcessExeFileName => "TrayApp.exe";
    }
}