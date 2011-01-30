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
using System.Diagnostics;

namespace Symbiote.Daemon.Host
{
    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName( int pid )
        {
            var processName = Process.GetProcessById( pid ).ProcessName;
            var processesByName = Process.GetProcessesByName( processName );
            string processIndexdName = null;

            for( var index = 0; index < processesByName.Length; index++ )
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter( "Process", "ID Process", processIndexdName );
                if ( ((int) processId.NextValue()).Equals( pid ) )
                {
                    return processIndexdName;
                }
            }
            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName( string indexedProcessName )
        {
            var parentId = new PerformanceCounter( "Process", "Creating Process ID", indexedProcessName );
            return Process.GetProcessById( (int) parentId.NextValue() );
        }

        public static Process Parent( this Process process )
        {
            return FindPidFromIndexedProcessName( FindIndexedProcessName( process.Id ) );
        }
    }
}