using ARXToKDRGold.Communication;
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
        private List<Users> LstUsers = new List<Users>();

        public ARXExporter()
        {
            InitializeComponent();
        }

        private Timer timer1 = null;

        public async Task onDebugAsync()
        {
            //OnStart(null);
            //ARXXml.GetDatafromARX();
            LstUsers.Clear();
            // write code her to do the job based on my requirements
            //GET DATA FROM ARX WEB REWUEST AUTH BASIC
            ARXXml.GetXML();
            LstUsers = ARXXml.GetList();
            LstUsers = ARXXml.CleanXML(LstUsers);

            //Library.WriteErrorLog("Timer ticked and som job as been done successfully");
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 10000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.WriteErrorLog("Test window service started");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            LstUsers.Clear();
            // write code her to do the job based on my requirements
            //GET DATA FROM ARX WEB REWUEST AUTH BASIC
#if DEBUG
            ARXXml.GetXML();
#else
            ARXXml.GetXMLData();
#endif
            LstUsers = ARXXml.GetList();
            LstUsers = ARXXml.CleanXML(LstUsers);
            ARXXml.SendxmlAsync(LstUsers);
            Library.WriteErrorLog("Timer ticked and som job as been done successfully");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Test window service stopped");
        }
    }
}