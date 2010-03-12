using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Hypertext
{
    public class HypertextRequest
    {
        private string _user = "";
        private string _password = "";
        private string _scheme = "";
        private string _hostname = "";
        private int _port;
        private List<string> _paths = new List<string>();
        private List<string> _variables = new List<string>();

        public static HypertextRequest Http()
        {
            return new HypertextRequest()
                   {
                       _scheme = "http"
                   };
        }

        public static HypertextRequest Http(string user, string password)
        {
            return new HypertextRequest()
               {
                   _scheme = "http",
                   _user = user,
                   _password = password
                };
        }

        public static HypertextRequest Https()
        {
            return new HypertextRequest()
            {
                _scheme = "https"
            };
        }

        public static HypertextRequest Https(string user, string password)
        {
            return new HypertextRequest()
            {
                _scheme = "https",
                _user = user,
                _password = password
            };
        }

        public HypertextRequest Host(string host)
        {
            _hostname = host;
            return this;
        }

        public HypertextRequest Path(string path)
        {
            _paths.Add(path);
            return this;
        }

        public HypertextRequest Port(int port)
        {
            _port = port;
            return this;
        }

        public HypertextRequest Variable<TValue>(string variableName, TValue value)
        {
            _variables.Add("{0}={1}".AsFormat(variableName, value));
            return this;
        }

        public void Post(Action<byte[]> process)
        {
            var request = WebRequest.Create(BuildURI());
            request.Method = WebRequestMethods.Http.Post;
            var response = request.GetResponse();

            var stream = response.GetResponseStream();
            var length = response.ContentLength;
            var bytes = new byte[length];
            stream.Write(bytes, 0, (int) length);
            process.BeginInvoke(bytes, null, null);
            response.Close();
        }



        private Uri BuildURI()
        {
            var pathBuilder = new DelimitedBuilder("/");
            _paths.ForEach(pathBuilder.Append);
            var path = _paths.ToString();

            var variableBuilder = new DelimitedBuilder("&");
            _variables.ForEach(variableBuilder.Append);
            var variables = variableBuilder.ToString();

            var format = variables != ""
                             ?
                                 "{0}?{1}".AsFormat(path, variables)
                             :
                                 "{0}".AsFormat(path);

            return new UriBuilder(_scheme, _hostname, _port, format).Uri;
        }
    }
}
