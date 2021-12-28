using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static object lockObject = new object();
        public static void LogException(Exception ex)
        {
            //Datetime:
            DateTime dateTime = DateTime.Now;
            // log exception details associated with datetime
            lock(lockObject)
            {
                File.AppendAllText("log.txt", $"{dateTime}: {ex.Message} at {ex.TargetSite}\n");
            }
        }
    }
}