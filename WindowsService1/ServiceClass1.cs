using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    using System.Drawing;
    using System.Windows.Forms;

    using ClassLibrary1;
    class ServiceClass1 : Class1
    {
      
        public override bool CheckOn {
            get
            {
                return !this.ScreenSaverRunning;
            }
        }

        protected override void SwitchOff()
        {
          LockWorkStation();
        }
    }
}
