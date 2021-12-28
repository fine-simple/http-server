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
        Dictionary<string, string> headerLines = new Dictionary<string, string>();
        int blankLineIndex;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string content;

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
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
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
            // get content if post request
            if(method == RequestMethod.POST)
            {
                for (int i = blankLineIndex + 1; i < requestLines.Length; i++)
                {
                    content += requestLines[i] + "\r\n";
                }
            }

            return true;
        }

        private bool ParseRequestLine()
        {
            string[] tokens = requestLines[0].Split(new string[] { " " }, StringSplitOptions.None);
            if (tokens.Length < 2)
                return false;
            // Check Method
            if (tokens[0] == "GET")
                method = RequestMethod.GET;
            else if (tokens[0] == "POST")
                method = RequestMethod.POST;
            else if (tokens[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;
            // Check URI
            if (!ValidateIsURI(tokens[1]))
                return false;
            relativeURI = tokens[1].Substring(1);
            //Check HTTP Version
            if (tokens.Length < 3 || tokens[2] == "HTTP/0.9" || tokens[2] == "undefined" || tokens[2] == "")
                httpVersion = HTTPVersion.HTTP09;
            else if (tokens[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (tokens[2] == "HTTP/1.1")
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
            try
            {
                for(int i = 1; requestLines[i] != ""; i++)
                {
                    string[] dict = requestLines[i].Split(new char[] { ':' }, 2);
                    if (dict.Length < 2)
                        return false;
                    headerLines[dict[0]] = dict[1];
                }
                if (httpVersion == HTTPVersion.HTTP11 && headerLines.ContainsKey("Host"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false; 
            }
        }

        private bool ValidateBlankLine()
        {
            for (int i = 1; i < requestLines.Length; i++)
            {
                if (requestLines[i] == "")
                {
                    blankLineIndex = i;
                    return true;
                }
            }
            return false;
        }
    }
}