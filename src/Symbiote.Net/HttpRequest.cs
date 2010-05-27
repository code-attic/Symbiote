using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public class HttpRequest
    {
        protected string _rawUrl;

        public string Verb { get; protected set; }
        public string Host { get { return Headers[HttpRequestHeader.Host]; } }
        public string Path { get; protected set; }
        public string Query { get; protected set; }
        public string RawUrl
        {
            get
            {
                _rawUrl = _rawUrl ?? "{0}://{1}{2}{3}".AsFormat(
                    Protocol,
                    Host,
                    Path,
                    Query != null ? "?{0}".AsFormat(Query) : ""
                                         );
                return _rawUrl;
            }
        }
        public string Protocol { get; protected set; }
        public Version Version { get; protected set; }
        public WebHeaderCollection Headers { get; protected set; }
        public Dictionary<string, string> QueryParameters { get; protected set; }
        public string Body { get; protected set; }

        public string UserAgent { get { return Headers[HttpRequestHeader.UserAgent]; } }
        public string Authentication { get { return Headers[HttpRequestHeader.Authorization]; } }

        public HttpRequest(string requestBody)
        {
            Protocol = "HTTP";
            Headers = new WebHeaderCollection();
            QueryParameters = new Dictionary<string, string>();
            Process(requestBody);
        }

        protected void Process(string requestBody)
        {
            try
            {
                // break into lines
                var lines = requestBody.Split(new string[] { "\r\n"}, StringSplitOptions.None);

                // handle each line with a method
                ProcessRequestLine(lines[0]);
                var lastLine = 1;
                IEnumerableExtenders.ForEach<string>(lines
                                         .Skip(1)
                                         .TakeWhile(x => !string.IsNullOrEmpty(x)), x =>
                                 {
                                     Headers.Add(x);
                                     lastLine++;
                                 });

                if(lastLine < lines.Length)
                {
                    Body = string.Join("\r\n", lines.Skip(lastLine+1));
                }
                
            }
            catch (Exception ex)
            {
                throw new HttpServerException("Cannot process poorly formed client request. \r\n {0}".AsFormat(requestBody));
            }
            
        }

        private void ProcessRequestLine(string requestLine)
        {
            var values = requestLine.Split(' ');
            var url = values[1].Split('?');
            var protocolAndVersion = values[2].Split('/');

            Verb = values[0];
            Path = url[0];
            Query = url.Length > 1 ? url[1] : null;
            Protocol = protocolAndVersion[0];
            Version = Version.Parse(protocolAndVersion[1]);
        }
    }
}