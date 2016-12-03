namespace lbsa71.TimedScreenLocker.Tuck
{
    using System;
    using System.Drawing;

    using lbsa71.TimedScreenLocker.Core;
    using lbsa71.TimedScreenLocker.Tuck.Properties;

    public class TuckEngine : TimedAppLauncher
    {
        protected override Icon AppIcon => Resources.appIcon;

        protected override string OtherProcessExeFileName => "TrayApp.exe";
    }
}