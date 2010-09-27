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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl
{
    class UnimplementedCommands
    {
        protected const string QUIT = "QUIT\r\n";
        protected const string KEY_TYPE = "TYPE {0}\r\n";
        protected const string RANDOM_KEY = "RANDOMKEY\r\n";
        protected const string GET_MANY = "MGET {0}\r\n";
        protected const string LIST_RANGE = "LRANGE {0} {1} {2}\r\n";
        protected const string RIGHT_PUSH = "RPUSH {0} {1}\r\n{2}\r\n";
        protected const string LIST_LENGTH = "LLEN {0}\r\n";
        protected const string GET_LIST_INDEX = "LINDEX {0} {1}\r\n";
        protected const string LEFT_POP = "LPOP {0}\r\n";
        protected const string ADD_TO_SET = "SADD {0} {1}\r\n";
        protected const string SET_CARDINALITY = "SCARD {0}\r\n";
        protected const string IS_MEMBER_OF_SET = "SISMEMBER {0} {1}\r\n";
        protected const string GET_MEMBER_OF_SET = "SMEMBERS {0}\r\n";
        protected const string GET_RANDOM_SET_MEMBER = "SRANDMEMBER {0}\r\n";
        protected const string POP_RANDOM_SET_MEMBER = "SPOP {0}\r\n";
        protected const string REMOVE_FROM_SET = "SREM {0} {1}\r\n";
        protected const string UNION_SETS = "SUNION {0}\r\n";
        protected const string STORE_SET_COMMANDS = "{0} {1} {2}\r\n";
        protected const string STORE_UNION_OFFSETS = "SUNIONSTORE";
        protected const string GET_SET_INTERSECTIONS = "SINTER {0}\r\n";
        protected const string SET_INTERSECTION_OFFSETS = "SINTERSTORE";
        protected const string GET_SET_DIFFERENCES = "SDIFF {0}\r\n";
        protected const string STORE_SET_DIFFERENCES = "SDIFFSTORE";
        protected const string MOVE_MEMBER_TO_SET = "SMOVE {0} {1} {2}\r\n";
    }
}
