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

using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;

namespace Symbiote.Log4Net
{
    public class Pattern
    {
        private const string _appDomain = "appdomain";
        private const string _date = "date";
        private const string _exception = "exception";
        private const string _file = "file";
        private const string _identity = "identity";
        private const string _location = "location";
        private const string _logger = "logger";
        private const string _level = "level";
        private const string _line = "line";
        private const string _message = "message";
        private const string _method = "method";
        private const string _newline = "newline";
        private const string _property = "property";
        private const string _timestamp = "timestamp";
        private const string _thread = "thread";
        private const string _type = "type";
        private const string _username = "username";

        #region utility

        private static string GetArgument(string arg, bool? leftJustify, int? width, int? limit)
        {
            var toLeft = leftJustify ?? false;
            var justifyChar = toLeft ? "-" : "";
            var min = width ?? 0;
            var minWidth = min == 0 ? "" : min.ToString();
            var max = limit ?? 0;
            var maxWidth = max == 0 ? "" : ".{0}".AsFormat(max.ToString());

            return "%{0}{1}{2}{3}"
                .AsFormat(justifyChar, minWidth, maxWidth, arg);
        }
        
        private DelimitedBuilder _builder = new DelimitedBuilder(" ");
        
        private Pattern Add(string argument)
        {
            _builder.Append(GetArgument(argument, null, null, null));
            return this;
        }
        private Pattern Add(string argument, bool? leftJustify, int? width, int? limit)
        {
            _builder.Append(GetArgument(argument, leftJustify, width, limit));
            return this;
        }
        public static Pattern New()
        {
            return new Pattern();
        }
        #endregion

        public Pattern AppDomain()
        {
            return Add(_appDomain);
        }
        public Pattern AppDomain(bool? leftJustify, int? width, int? limit)
        {
            return Add(_appDomain, leftJustify, width, limit);
        }
        public Pattern Date()
        {
            return Add(_date);
        }
        public Pattern Date(bool? leftJustify, int? width, int? limit)
        {
            return Add(_date, leftJustify, width, limit);
        }
        public Pattern Exception()
        {
            return Add(_exception);
        }
        public Pattern Exception(bool? leftJustify, int? width, int? limit)
        {
            return Add(_exception, leftJustify, width, limit);
        }

        public Pattern File()
        {
            return Add(_file);
        }
        public Pattern File(bool? leftJustify, int? width, int? limit)
        {
            return Add(_file, leftJustify, width, limit);
        }

        public Pattern Identity()
        {
            return Add(_identity);
        }
        public Pattern Identity(bool? leftJustify, int? width, int? limit)
        {
            return Add(_identity, leftJustify, width, limit);
        }

        public Pattern Location()
        {
            return Add(_location);
        }
        public Pattern Location(bool? leftJustify, int? width, int? limit)
        {
            return Add(_location, leftJustify, width, limit);
        }

        public Pattern Logger()
        {
            return Add(_logger);
        }
        public Pattern Logger(bool? leftJustify, int? width, int? limit)
        {
            return Add(_logger, leftJustify, width, limit);
        }

        public Pattern Level()
        {
            return Add(_level);
        }
        public Pattern Level(bool? leftJustify, int? width, int? limit)
        {
            return Add(_level, leftJustify, width, limit);
        }

        public Pattern Line()
        {
            return Add(_line);
        }
        public Pattern Line(bool? leftJustify, int? width, int? limit)
        {
            return Add(_line, leftJustify, width, limit);
        }

        public Pattern Message()
        {
            return Add(_message);
        }
        public Pattern Message(bool? leftJustify, int? width, int? limit)
        {
            return Add(_message, leftJustify, width, limit);
        }

        public Pattern Method()
        {
            return Add(_method);
        }
        public Pattern Method(bool? leftJustify, int? width, int? limit)
        {
            return Add(_method, leftJustify, width, limit);
        }

        public Pattern Newline()
        {
            return Add(_newline);
        }

        public Pattern Property()
        {
            return Add(_property);
        }
        public Pattern Property(bool? leftJustify, int? width, int? limit)
        {
            return Add(_property, leftJustify, width, limit);
        }

        public Pattern TimeStamp()
        {
            return Add(_timestamp);
        }
        public Pattern TimeStamp(bool? leftJustify, int? width, int? limit)
        {
            return Add(_timestamp, leftJustify, width, limit);
        }

        public Pattern Thread()
        {
            return Add(_thread);
        }
        public Pattern Thread(bool? leftJustify, int? width, int? limit)
        {
            return Add(_thread, leftJustify, width, limit);
        }

        public Pattern Type()
        {
            return Add(_type);
        }
        public Pattern Type(bool? leftJustify, int? width, int? limit)
        {
            return Add(_type, leftJustify, width, limit);
        }
        
        public Pattern UserName()
        {
            return Add(_username);
        }
        public Pattern UserName(bool? leftJustify, int? width, int? limit)
        {
            return Add(_username, leftJustify, width, limit);
        }

        public string ToString()
        {
            return _builder.ToString();
        }
    }
}