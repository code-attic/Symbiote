using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;

namespace Symbiote.Http.Config
{
    public class HttpListenerConfigurator
    {
        private HttpListenerConfiguration _configuration;

        public HttpListenerConfigurator AddPort(int port)
        {
            _configuration.Ports.Add(port);
            return this;
        }

        public HttpListenerConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpListenerConfigurator(HttpListenerConfiguration configuration)
        {
            _configuration = configuration;
            _configuration.SelfHosted = true;
        }
    }
}
