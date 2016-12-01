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
    using System.CodeDom.Compiler;
    using System.IO;

    public partial class Service1 : ServiceBase
    {
        private ServiceClass1 serviceClass1;

        public Service1()
        {
            InitializeComponent();

            this.serviceClass1 = new ServiceClass1();

       

            this.serviceClass1.Log("Service constructed.");
        }

        protected override void OnStart(string[] args)
        {
            this.serviceClass1.Log("Service OnStart.");
            this.serviceClass1.Start();
        }

        protected override void OnStop()
        {
            this.serviceClass1.Log("Service OnStop.");
            this.serviceClass1.Stop();
        }

        protected override void OnContinue()
        {
            this.serviceClass1.Log("Service OnContinue.");
            base.OnContinue();
            this.serviceClass1.Start();
        }

        protected override void OnPause()
        {
            this.serviceClass1.Log("Service OnPause.");
            base.OnPause();
            this.serviceClass1.Stop();
        }

        protected override void OnShutdown()
        {
            this.serviceClass1.Log("Service OnShutdown.");
            base.OnShutdown();
            this.serviceClass1.Stop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            this.serviceClass1.Log("Service OnSessionChange:" + changeDescription.Reason);

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
