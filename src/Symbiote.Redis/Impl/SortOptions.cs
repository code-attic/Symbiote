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

namespace Symbiote.Redis.Impl
{
    public class SortOptions
    {
        public string Key { get; set; }
        public bool Descending { get; set; }
        public bool Lexographically { get; set; }
        public Int32 LowerLimit { get; set; }
        public Int32 UpperLimit { get; set; }
        public string By { get; set; }
        public string StoreInKey { get; set; }
        public string Get { get; set; }

        public string ToCommand()
        {
            var command = "SORT " + Key;
            if ( LowerLimit != 0 || UpperLimit != 0 )
                command += " LIMIT " + LowerLimit + " " + UpperLimit;
            if ( Lexographically )
                command += " ALPHA";
            if ( !string.IsNullOrEmpty( By ) )
                command += " BY " + By;
            if ( !string.IsNullOrEmpty( Get ) )
                command += " GET " + Get;
            if ( !string.IsNullOrEmpty( StoreInKey ) )
                command += " STORE " + StoreInKey;
            return command;
        }
    }
}