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
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Host
{
    public class ConsoleHost
        : IHost
    {
        public readonly ServiceName Name;
        public readonly IServiceCoordinator ServiceCoordinator;

        #region IHost Members

        public void Start()
        {
            CheckToSeeIfWinServiceRunning();
            "Daemon loading in console...".ToDebug<IHost>();
            var externalTriggeredTerminatation = new ManualResetEvent( false );
            var waitHandles = new WaitHandle[] {externalTriggeredTerminatation};

            Console.CancelKeyPress += delegate
                                          {
                                              "Exiting.".ToInfo<IHost>();
                                              ServiceCoordinator.Stop();
                                              ServiceCoordinator.Dispose();
                                              externalTriggeredTerminatation.Set();
                                          };

            ServiceCoordinator.Start();
            "Daemon started, press Control+C to exit."
                .ToInfo<IHost>();
            WaitHandle.WaitAny( waitHandles );
        }

        #endregion

        protected void CheckToSeeIfWinServiceRunning()
        {
            //if (ServiceController.GetServices().Where(s => s.ServiceName == Name.FullName).Any())
            //{
            //    "There is an instance of this {0} running as a windows service"
            //        .ToWarn<IHost>(Name);
            //}
        }

        public ConsoleHost( ServiceName name, IServiceCoordinator serviceCoordinator )
        {
            Name = name;
            ServiceCoordinator = serviceCoordinator;
        }
    }
}