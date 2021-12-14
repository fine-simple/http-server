using System;
using System.Collections.Generic;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            contentLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
                return false;
            // Parse Request line
            if (!ParseRequestLine())
                return false;
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;
            return true;
        }

        private bool ParseRequestLine()
        {
            requestLines = contentLines[0].Split(' ');
            if (requestLines.Length < 3)
                return false;
            // Check Method
            if (requestLines[0] == "GET")
                method = RequestMethod.GET;
            else if (requestLines[0] == "POST")
                method = RequestMethod.POST;
            else if (requestLines[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;
            // Check URI
            if (!ValidateIsURI(requestLines[1]))
                return false;
            relativeURI = requestLines[1].Substring(1);
            //Check HTTP Version
            if (requestLines[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else if (requestLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                return false;
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            try
            {
                for (int i = 1; contentLines[i] != ""; i++)
                {
                    string[] dict = contentLines[i].Split(':');
                    if (dict.Length < 2)
                        continue;
                    headerLines[dict[0]] = dict[1];
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false; 
            }
        }

        private bool ValidateBlankLine()
        {
            for (int i = 2; i < contentLines.Length; i++)
            {
                if (contentLines[i] == "")
                    return true;
            }
            return false;
        }

    }
}