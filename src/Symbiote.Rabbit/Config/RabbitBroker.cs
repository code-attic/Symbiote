// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using RabbitMQ.Client;

namespace Symbiote.Rabbit.Config
{
    public class RabbitBroker : IRabbitBroker
    {
        protected ConnectionFactory _factory { get; set; }
        protected IConnection Connection { get; set; }
        public int BalanceGroup { get; set; }
        public static string ResponseId { get; set; }

        protected ConnectionFactory Factory
        {
            get
            {
                _factory = _factory ?? new ConnectionFactory
                                           {
                                               HostName = Address,
                                               Port = Port,
                                               UserName = User,
                                               Password = Password,
                                               VirtualHost = VirtualHost,
                                               Protocol = Protocols.Lookup( Protocol ),
                                               RequestedFrameMax = uint.MaxValue,
                                               RequestedHeartbeat = ushort.MaxValue
                                           };

                return _factory;
            }
        }

        public string Name { get; set; }
        public string Protocol { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }

        public IConnection GetConnection()
        {
            if ( Connection == null || !Connection.IsOpen )
            {
                Connection = Factory.CreateConnection();
            }
            return Connection;
        }

        public void SetDefaults()
        {
            Name = "default";
            Protocol = "AMQP_0_9_1";
            User = "guest";
            Password = "guest";
            VirtualHost = "/";
            Address = "localhost";
            Port = 5672;
            ResponseId = ResponseId ?? Name + AppDomain.CurrentDomain.Id;
        }

        public RabbitBroker( string address, int port ) : this()
        {
            Address = address;
            Port = port;
        }

        public RabbitBroker()
        {
            SetDefaults();
        }
    }
}