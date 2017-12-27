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
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endp = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(endp);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(3000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket nc = serverSocket.Accept();
                Thread NcThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                NcThread.Start(nc);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket client_socket = (obj as Socket);
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            client_socket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.

            while (true)
            {
                try
                {
                    byte[] ReqData = new byte[1024 * 1024];
                    // TODO: Receive request
                    int len =   client_socket.Receive(ReqData);
                    // TODO: break the while loop if receivedLen==0
                            if (len == 0) break;
                    // TODO: Create a Request object using received request string
                    string x = Encoding.ASCII.GetString(ReqData);
                    Request clientR = new Request(x);
                    // TODO: Call HandleRequest Method that returns the response
                    Response ServerResponse= HandleRequest(clientR);
                    // TODO: Send Response back to client
                    client_socket.Send(Encoding.ASCII.GetBytes(ServerResponse.ResponseString));

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);

                }
            }

            // TODO: close client socket
            client_socket.Close();
        }

        Response HandleRequest(Request request)
        {
            // throw new NotImplementedException();
            //string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                    return new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string Physical_Path = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string RedirectionPagePath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (RedirectionPagePath != "")
                    return new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(Configuration.RedirectionDefaultPageName), RedirectionPagePath);
                //TODO: check file exists
                if (!File.Exists(Physical_Path))
                    return new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), "");

                //TODO: read the physical file
                StreamReader R = new StreamReader(Physical_Path);
                string DataFile = R.ReadToEnd();
                R.Close();
                // Create OK response
                // Head
                if(request.BoolMethodHead )
                    return new Response(StatusCode.OK, "text/html", "", "");
                
                // Get
                return new Response(StatusCode.OK, "text/html", DataFile,"");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error. 
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            bool checkRed = Configuration.RedirectionRules.ContainsKey(relativePath);
            if (checkRed)
                return Configuration.RedirectionRules[relativePath];

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("Default Page " + defaultPageName + " doesn't exist"));
                return string.Empty;
            }
            // else read file and return its content

            StreamReader R = new StreamReader(filePath);
            string Page = R.ReadToEnd();
            R.Close();
            return Page;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                StreamReader R = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!R.EndOfStream)
                {
                    string Data = R.ReadLine();
                    string[] result = Data.Split(',');
                    Configuration.RedirectionRules.Add(result[0], result[1]);
                }
                R.Close();
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
