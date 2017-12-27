using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // throw new NotImplementedException();
            this.code = code;
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("contentType: "+contentType);
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("Date: " + DateTime.Now);
            headerLines.Add("Server: " + Configuration.ServerType);
            if(redirectoinPath != "" )
            headerLines.Add("location: " + redirectoinPath);

            // TODO: Create the request string
            responseString += GetStatusLine(code);

            foreach(var line in headerLines)
                responseString += (line + "\r\n");

            responseString += ("\r\n" + content);
        }

        private string GetStatusLine(StatusCode code)
        {
            
            // TODO: Create the response status line and return it
            //  string statusLine = "HTTP/1.1 "+ ((int)code).ToString()+" "+ code.ToString()+ " \r\n";
            string statusLine = "HTTP/1.1 "+ ((int)code).ToString()+" "+code.ToString() + "\r\n";
            return statusLine;
        }
    }
}
