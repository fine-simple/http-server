using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            //Datetime:
            DateTime dateTime = DateTime.Now;
            // log exception details associated with datetime 
            File.AppendAllText("log.txt", $"{dateTime}: {ex.Message} at {ex.TargetSite}\n");
        }
    }
}