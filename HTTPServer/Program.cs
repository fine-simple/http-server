using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000, "redirectionRules.txt");
            // 2) Start Server
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            FileStream fs = File.Create("redirectionRules.txt");
            string aboutRedirect = "aboutus.html,aboutus2.html";
            fs.Write(ASCIIEncoding.ASCII.GetBytes(aboutRedirect), 0, aboutRedirect.Length);
            fs.Close();
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}