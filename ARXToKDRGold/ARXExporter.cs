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
        public static string exportType;
        private double checktime;

        public void ReadDBSettings()
        {
            try
            {
                //string appPath = System.Windows.Forms.Application.StartupPath;
                string[] lines = System.IO.File.ReadAllLines("C:\\KDRGoldExport\\settings.txt");
                char demiliter = '=';
                //char semicolon = ';';
                //is LOCKED

                exportType = lines[0].Split(demiliter)[1];
                double.TryParse(lines[1].Split(demiliter)[1], out checktime);
                //ProgTittle = lines[3].Split(demiliter)[1];
                ////DATA
                //dataSource = lines[5].Split(demiliter)[1];
                //catalog = lines[6].Split(demiliter)[1];
                //var lstSpnames = lines[7].Split(demiliter)[1];
                //var spltlstSpnames = lstSpnames.Split(semicolon);
                //foreach (var sp in spltlstSpnames)
                //{
                //    sp_names.Add(sp);
                //}

                //filePath = lines[8].Split(demiliter)[1];
                //fileName = lines[9].Split(demiliter)[1];
                //decimalCount = int.Parse(lines[10].Split(demiliter)[1]);

                ////EMAIL - SETTINGS
                //SMTP = lines[12].Split(demiliter)[1];
                //eUser = lines[13].Split(demiliter)[1];
                //ePwd = lines[14].Split(demiliter)[1];
                //SMTPport = int.Parse(lines[15].Split(demiliter)[1]);

                ////EMAIL
                //Subject = lines[16].Split(demiliter)[1];
                //bodyHead = lines[17].Split(demiliter)[1];
                //bodyMain = lines[18].Split(demiliter)[1];
                //bodySignature = lines[19].Split(demiliter)[1];
                //if (!Directory.Exists(filePath))
                //{
                //    Directory.CreateDirectory(filePath);
                //}

                //foreach (string line in lines)
                //{
                //    if (isEmail)
                //    {
                //        emailList.Add(line);
                //    }
                //    else
                //    //Read the line and do the task
                //    if (line.ToLower().Equals("[email]"))
                //    {
                //        isEmail = true;
                //    }
                //}

                //if (isLocked.ToLower().Equals("yes"))
                //{
                //    BisLocked = true;
                //}
            }
            catch (Exception e)
            {
                Library.WriteErrorLog("Error" + e);
            }
        }

        public async Task OnDebugAsync()
        {
            //OnStart(null);
            //ARXXml.GetDatafromARX();

            // write code her to do the job based on my requirements
            //GET DATA FROM ARX WEB REWUEST AUTH BASIC

            ReadDBSettings();
            LstUsers.Clear();
            ARXXml.GetXML();
            LstUsers = ARXXml.GetList();
            LstUsers = ARXXml.CleanXML(LstUsers);
            await ARXXml.SendxmlAsync(LstUsers);

            //Library.WriteErrorLog("Timer ticked and som job as been done successfully");
        }

        protected override void OnStart(string[] args)
        {
            ReadDBSettings();
            timer1 = new Timer();

            //this.timer1.Interval = 300000;
            this.timer1.Interval = checktime * 60000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_TickAsync);
            timer1.Enabled = true;
            Library.WriteErrorLog("Test window service started");
        }

        private async void timer1_TickAsync(object sender, ElapsedEventArgs e)
        {
            ReadDBSettings();
            LstUsers.Clear();
            // write code her to do the job based on my requirements
            //GET DATA FROM ARX WEB REWUEST AUTH BASIC
            ARXXml.GetXML();
            LstUsers = ARXXml.GetList();
            LstUsers = ARXXml.CleanXML(LstUsers);
            await ARXXml.SendxmlAsync(LstUsers);
            Library.WriteErrorLog("Timer ticked and som job as been done successfully");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Test window service stopped");
        }
    }
}