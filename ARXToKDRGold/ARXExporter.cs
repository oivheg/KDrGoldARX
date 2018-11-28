using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ARXToKDRGold
{
    public partial class ARXExporter : ServiceBase
    {
        public ARXExporter()
        {
            InitializeComponent();
        }

        private Timer timer1 = null;

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 30000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.WriteErrorLog("Test window service started");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            // write code her to do the job based on my requirements
            //GET DATA FROM ARX WEB REWUEST AUTH BASIC
            ARXXml.GetXML();
            ARXXml.Sendxml();
            Library.WriteErrorLog("Timer ticked and som job as been done successfully");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Test window service stopped");
        }
    }
}