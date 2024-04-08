using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static OzdocsMobileWebAPI.BusinessLayer.BusinessTier;

namespace OzdocsMobileWebAPI.CreateLogFiles
{
    public class CreateLog
    {
        private string sLogFormat;
        private string sErrorTime;

        public CreateLog()
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            //[02-03-2022-14:05:28] Cmdline: ALLPRT MEMF01 00000001 PRINTER 
            sLogFormat = "[" + DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + "] ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            //DateTime.Now.ToString("MM/dd/yyyy")
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.ToString("MM");// DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.ToString("dd");
            sErrorTime = sYear + sMonth + sDay;
        }

        public void CreateLogFile(string sPathName, string sLogMsg)
        {
            if(!Directory.Exists(sPathName))
            {
                Directory.CreateDirectory(sPathName);
            }
            StreamWriter sw = new StreamWriter(sPathName + "\\"  + "OzdocsMobileWebAPI" + ".log", true);

            if (sLogMsg != string.Empty) { sw.WriteLine(sLogFormat + sLogMsg); }
            else { sw.WriteLine(""); }

            sw.Flush();
            sw.Close();
        }
    }


}