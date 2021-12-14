using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.LoadRedirectionRules(redirectionMatrixPath);
            
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);

            this.serverSocket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                // start thread for each accepted connection.
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            Socket clientSocket = (Socket) obj;
            // infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            while (true)
            {
                try
                {
                    // Receive request
                    byte[] buffer = new byte[1024 * 1024];
                    int receivedLen = clientSocket.Receive(buffer);
                    // break the while loop if recieved nothing
                    if (receivedLen == 0)
                        break;
                    
                    Request request = new Request(ASCIIEncoding.ASCII.GetString(buffer, 0, receivedLen));
                    
                    // Send Response
                    Response response = HandleRequest(request);
                    clientSocket.Send(ASCIIEncoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content = string.Empty;
            string redirected = string.Empty;
            StatusCode statusCode = StatusCode.OK;
            try
            {
                // check for bad request 
                if (request.ParseRequest() == false)
                    statusCode = StatusCode.BadRequest;
                
                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                
                // check for redirect
                redirected = GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirected != string.Empty)
                {
                    statusCode = StatusCode.Redirect;
                    physicalPath = Path.Combine(Configuration.RootPath, Configuration.RedirectionDefaultPageName);
                }

                // check file
                if (!File.Exists(physicalPath))
                {
                    if (redirected != string.Empty)
                    {
                        statusCode = StatusCode.InternalServerError;
                        physicalPath = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                    }
                    else
                    {
                        statusCode = StatusCode.NotFound;
                        physicalPath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                    }
                }
                
                // read the physical file
                content = LoadDefaultPage(physicalPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                // in case of exception, return Internal Server Error.
                statusCode = StatusCode.InternalServerError;
            }
            // Create OK response
            return new Response(statusCode, "text/html", content, redirected);
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            Configuration.RedirectionRules = new Dictionary<string, string>();
            try
            {
                // read the redirection rules from file
                string rawText = File.ReadAllText(filePath);
                // fill Configuration.RedirectionRules dictionary
                string[] redirections = rawText.Split('\n');
                for (int i = 0; i < redirections.Length; i++)
                {
                    string[] redirectionArr = redirections[i].Split(',');
                    Configuration.RedirectionRules.Add(redirectionArr[0], redirectionArr[1]);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}