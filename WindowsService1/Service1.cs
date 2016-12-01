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

            var logFile = AppDomain.CurrentDomain.BaseDirectory + "service.log";

            var writer = new StreamWriter(logFile);

            
            Console.SetOut(writer);

            Console.WriteLine("Service constructed.");
        }

        protected override void OnStart(string[] args)
        {
            Console.WriteLine("Service OnStart.");
            this.serviceClass1.Start();
        }

        protected override void OnStop()
        {
            Console.WriteLine("Service OnStop.");
            this.serviceClass1.Stop();
        }

        protected override void OnContinue()
        {
            Console.WriteLine("Service OnContinue.");
            base.OnContinue();
            this.serviceClass1.Start();
        }

        protected override void OnPause()
        {
            Console.WriteLine("Service OnPause.");
            base.OnPause();
            this.serviceClass1.Stop();
        }

        protected override void OnShutdown()
        {
            Console.WriteLine("Service OnShutdown.");
            base.OnShutdown();
            this.serviceClass1.Stop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Console.WriteLine("Service OnSessionChange:" + changeDescription.Reason);

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
