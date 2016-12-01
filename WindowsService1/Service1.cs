using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private ServiceClass1 serviceClass1;

        public Service1()
        {
            InitializeComponent();

            this.serviceClass1 = new ServiceClass1();
        }

        protected override void OnStart(string[] args)
        {
            this.serviceClass1.Start();
        }

        protected override void OnStop()
        {
            this.serviceClass1.Stop();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            this.serviceClass1.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.serviceClass1.Stop();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            this.serviceClass1.Stop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    this.serviceClass1.ScreenTurnedOn();
                    break;
                case SessionChangeReason.SessionLogoff:
                    this.serviceClass1.ScreenTurnedOff();
                    break;
                case SessionChangeReason.SessionLock:
                    this.serviceClass1.ScreenTurnedOff();
                    break;
                case SessionChangeReason.SessionUnlock:
                    this.serviceClass1.ScreenTurnedOn();
                    break;
            }

            base.OnSessionChange(changeDescription);
        }
    }
}
