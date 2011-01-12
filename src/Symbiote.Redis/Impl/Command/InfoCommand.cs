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

using System.Collections.Generic;
using System.Text;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class InfoCommand
        : RedisCommand<Dictionary<string, string>>
    {
        protected const string INFO = "INFO\r\n";

        public Dictionary<string,string> GetInfo(IConnection connection)
        {
            byte[] r = connection.SendExpectData(null, INFO);
            var dict = new Dictionary<string, string>();

            foreach (var line in Encoding.UTF8.GetString(r).Split('\n'))
            {
                int p = line.IndexOf(':');
                if (p == -1)
                    continue;
                dict.Add(line.Substring(0, p), line.Substring(p + 1));
            }
            return dict;
        }

        public InfoCommand()
        {
            Command = GetInfo;
        }
    }
}