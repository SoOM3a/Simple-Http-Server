using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public bool BoolMethodHead {
            get
            {
                if (method == RequestMethod.HEAD)
                    return true;
                else
                    return false;
            }
        }
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
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //   throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = this.requestString.Split(new String[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) return false;
            // Parse Request line
            if (!ParseRequestLine()) return false;
            // Validate blank line exists
            if (!ValidateBlankLine()) return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines()) return false;
            if(method == RequestMethod.POST)
              contentLines = requestLines[requestLines.Length - 1].Split(new String[] { "&" }, StringSplitOptions.None);
            
            return true;
        }

        private bool ParseRequestLine()
        {
            //  throw new NotImplementedException();
            String[] FLine = requestLines[0].Split(' ');
            if (FLine.Length < 3) return false;

            try
            {
                method = (RequestMethod)Enum.Parse(typeof(RequestMethod), FLine[0]);
            }
            catch (Exception e)
            {
                return false;
            }
         
            relativeURI = FLine[1].Substring(1);
            if (FLine[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (FLine[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (FLine[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;

            if (!ValidateIsURI(relativeURI)) return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            // get /uri httpversion // ""
            for(int i = 1;  i<requestLines.Length-2;i++)
            {
                String[] HLine = requestLines[i].Split(new String[] { ": "}, StringSplitOptions.None);
                if (HLine.Length < 2) return false;

                HeaderLines.Add(HLine[0], HLine[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            if (requestLines[requestLines.Length - 2] == "")
                return true;

            return false;
        }

    }
}
