/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using org.apache.qpid.client;

namespace Symbiote.Qpid.Config
{
    public class QpidBroker :
        IQpidBroker
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        protected IClient Client { get; set; }
        public IClient GetClient()
        {
            Client.Connect(Address, Port, VirtualHost, User, Password);
            return Client;
        }

        public QpidBroker()
        {
            Client = new Client();
        }
    }
}