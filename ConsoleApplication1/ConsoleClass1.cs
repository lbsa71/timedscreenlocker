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

                Console.WriteLine("State on:" + screenOn);

                return screenOn;
            }
        }

        protected override void SetText(string text)
        {
            Console.WriteLine(text);
        }

        protected override void SwitchOff()
        {
            Console.WriteLine("Locking Screen.");

            // LockWorkStation();
        }
    }
}