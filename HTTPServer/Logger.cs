using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace HTTPServer
{
    class Logger
    {
        static Semaphore fileSemaphore = new Semaphore(1,1);
        public static void LogException(Exception ex)
        {
            //Datetime:
            DateTime dateTime = DateTime.Now;
            // log exception details associated with datetime
            fileSemaphore.WaitOne();
            File.AppendAllText("log.txt", $"{dateTime}: {ex.Message} at {ex.TargetSite}\n");
            fileSemaphore.Release();
        }
    }
}