using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayApp
{
    using ClassLibrary1;

    public class TrayNotification : Class1
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
