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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Symbiote.Core.Futures;
using Symbiote.Http.Impl.Adapter.TcpListener;
using Symbiote.Http.Owin;
using Timer = System.Timers.Timer;

namespace Symbiote.Http.Impl.Adapter.SocketListener {
    public class SocketServer :
        IHost, IDisposable
    {
        public IHttpServerConfiguration Configuration { get; set; }
        public IRouteRequest RequestRouter { get; set; }
        public bool Running { get; set; }
        public IPAddress ServerAddress { get; set; }
        public IPEndPoint ServerEndpoint { get; set; }
        public Socket HttpSocket { get; set; }
        public int Connections { get; set; }
        public int Disconnects { get; set; }
        public Timer Timer { get; set; }
        public ClientSocketNode Root { get; set; }
        public List<Task> Tasks { get; set; }

        public void Listen()
        {
            Future
                .Of( () => HttpSocket.BeginAccept( OnClient, null ) )
                .Start();
        }

        public void OnClient( IAsyncResult result )
        {
            try
            {
                Connections ++;
                var client = HttpSocket.EndAccept( result );
                Listen();
                var adapter = new ClientSocketAdapter( client, RemoveClient, OnContext );
                Root.Add( adapter );
            }
            finally
            {
                Listen();
            }
            //new ClientSocketAdapter( client, RemoveClient, OnContext );
        }

        public void RemoveClient( string id )
        {
            Disconnects ++;
        }

        public void OnContext( IContext context )
        {
            context.SpawnApplication( OnApplicationException );
        }

        public void OnApplicationException( Exception exception )
        {
            Console.WriteLine( "Well, this is bad: \r\n {0}", exception );
        }

        public void Start()
        {
            HttpSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            HttpSocket.Bind( ServerEndpoint );
            HttpSocket.Listen( 5000 );
            Running = true;
            Tasks.Add( SpawnTask() );
            Listen();
        }

        public void Stop()
        {
            HttpSocket.Close();
        }


        public void Process()
        {
            var node = Root;
            while(Running)
            {
                node.Process();
                node = node.Next;
                Thread.Sleep(0);
            }
        }

        public Task SpawnTask()
        {
            var task = new Task( Process, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness );
            task.Start();
            return task;
        }


        public SocketServer( IHttpServerConfiguration configuration, IRouteRequest router )
        {
            Configuration = configuration;
            ServerAddress = IPAddress.Any;
            ServerEndpoint = new IPEndPoint( ServerAddress, configuration.Port );
            RequestRouter = router;
            Timer = new Timer(1000);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Enabled = true;
            Timer.Start();
            Tasks = new List<Task>();
            Root = new ClientSocketNode();
        }

        void Timer_Elapsed(object sender,ElapsedEventArgs e) 
        {
            Console.WriteLine( "{0} - {1}", Connections, Disconnects );
        }

        public void Dispose()
        {
            Timer.Enabled = false;
            Timer.Stop();
            Timer.Elapsed -= Timer_Elapsed;
            if( HttpSocket.Connected )
                Stop();
        }
    }
}
