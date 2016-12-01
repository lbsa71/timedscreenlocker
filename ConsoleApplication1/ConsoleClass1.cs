namespace ConsoleApplication1
{
    using System;

    using ClassLibrary1;

    internal class ConsoleClass1 : Class1
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

        protected override void SetText(string text)
        {
            this.Log(text);
        }

        protected override void SwitchOff()
        {
            this.Log("Locking Screen.");

            // LockWorkStation();
        }
    }
}