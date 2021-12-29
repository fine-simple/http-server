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
            // create the rules of redirection 
            CreateRedirectionRulesFile();
            // Start server on port 1000
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            /*
             * Create redirectionRules.txt
             * each line in the file specify a redirection rule
             * example: "aboutus.html,aboutus2.html"
             * means that when making request to aboustus.html,, it redirects me to aboutus2
            */
            const string filepath = "redirectionRules.txt";
            if (File.Exists(filepath))
                return;
            
            string[] initialRedirections = {
                "aboutus.html,aboutus2.html"
            };
            File.WriteAllLines(filepath, initialRedirections);
        }
         
    }
}