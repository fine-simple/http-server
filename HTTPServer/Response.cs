using System;
using System.Collections.Generic;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        Redirect = 301,
        BadRequest = 400,
        NotFound = 404,
        InternalServerError = 500
    }

    class Response
    {
        string responseString;
        List<string> headerLines;

        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines = new List<string>() { 
                $"Content-Type: {contentType}",
                $"Content-Length: {content.Length}",
                $"Date: {DateTime.Now}",
            };
            
            if (redirectoinPath != string.Empty)
                headerLines.Add($"Location: {redirectoinPath}");
            // TODO: Create the response string
            
            // status line
            responseString = $"{GetStatusLine(code)}\r\n";
            // headers
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += $"{headerLines[i]}\r\n";
            }
            // Blank Line
            responseString += "\r\n";
            // Content
            responseString += content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine;
            string statusMsg = string.Empty;
            switch (code)
            {
                case StatusCode.OK:
                    statusMsg = "OK";
                    break;
                case StatusCode.Redirect:
                    statusMsg = "Moved Permanently";
                    break;
                case StatusCode.BadRequest:
                    statusMsg = "Bad Request";
                    break;
                case StatusCode.NotFound:
                    statusMsg = "Not Found";
                    break;
                case StatusCode.InternalServerError:
                    statusMsg = "Internal Server Error";
                    break;
            }
            
            statusLine = $"HTTP/1.1 {(int)code} {statusMsg}";

            return statusLine;
        }

        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
    }
}
