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
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);

            this.serverSocket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket clientSocket = (Socket) obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] buffer = new byte[1024 * 1024];
                    int len = clientSocket.Receive(buffer);
                    // TODO: break the while loop if receivedLen==0
                    if (len == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    Request request = new Request(ASCIIEncoding.ASCII.GetString(buffer));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    //clientSocket.Send(ASCIIEncoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content = "";
            StatusCode statusCode = StatusCode.OK;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                    statusCode = StatusCode.BadRequest;
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI.Substring(1));
                //TODO: check for redirect
                string redirected = GetRedirectionPagePathIFExist(request.relativeURI.Substring(1));
                if (redirected != string.Empty)
                    statusCode = StatusCode.Redirect;
                //TODO: check file exists
                if (File.Exists(Path.Combine(Configuration.RootPath, redirected)))
                    physicalPath = Path.Combine(Configuration.RootPath, redirected);
                //TODO: read the physical file
                content = LoadDefaultPage(physicalPath);
                // Create OK response
                return new Response(statusCode, "text/html", content, redirected);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                statusCode = StatusCode.InternalServerError;
                return new Response(statusCode, "text/html", content, "");
            }
            
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

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
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
                // TODO: using the filepath paramter read the redirection rules from file
                string rawText = File.ReadAllText(filePath);
                // then fill Configuration.RedirectionRules dictionary
                string[] redirections = rawText.Split('\n');

                for (int i = 0; i < redirections.Length; i++)
                {
                    string[] redirectionArr = redirections[i].Split(',');
                    Configuration.RedirectionRules.Add(redirectionArr[0], redirectionArr[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}