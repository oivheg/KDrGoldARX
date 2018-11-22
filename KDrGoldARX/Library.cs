using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDrGoldARX
{
    public static class Library
    {
        public static void WriteErrorLog(Exception ex)
        {
            LogMSG(ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
        }

        public static void WriteErrorLog(string Message)
        {
            LogMSG(Message);
        }

        private static void LogMSG(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                throw;
            }
        }
    }
}