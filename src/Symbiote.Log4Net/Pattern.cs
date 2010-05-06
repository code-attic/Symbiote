using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Log4Net
{
    public class Pattern
    {
        private static readonly string _appDomain = "appdomain";
        private static readonly string _date = "date";
        private static readonly string _exception = "exception";
        private static readonly string _file = "file";
        private static readonly string _identity = "identity";
        private static readonly string _location = "location";
        private static readonly string _logger = "logger";
        private static readonly string _level = "level";
        private static readonly string _line = "line";
        private static readonly string _message = "message";
        private static readonly string _method = "method";
        private static readonly string _newline = "newline";
        private static readonly string _property = "property";
        private static readonly string _timestamp = "timestamp";
        private static readonly string _thread = "thread";
        private static readonly string _type = "type";
        private static readonly string _username = "username";

        #region utility

        private static string GetArgument(string arg, int? min, int? max)
        {
            if(min != null && max != null)
                return "%{0}.{1}{2}".AsFormat(min, max, arg);
            else if(min != null)
                return "%.{0}{1}".AsFormat(min, arg);
            else if(max != null)
                return "%{0}{1}".AsFormat(max, arg);
            return "%{0}".AsFormat(arg);
        }
        
        private DelimitedBuilder _builder = new DelimitedBuilder(" ");
        
        private Pattern Add(string argument)
        {
            _builder.Append(GetArgument(argument, null, null));
            return this;
        }
        private Pattern Add(string argument, int min, int max)
        {
            _builder.Append(GetArgument(argument, min, max));
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
        public Pattern AppDomain (int min, int max)
        {
            return Add(_appDomain, min, max);
        }
        public Pattern Date()
        {
            return Add(_date);
        }
        public Pattern Date(int min, int max)
        {
            return Add(_date, min, max);
        }
        public Pattern Exception()
        {
            return Add(_exception);
        }
        public Pattern Exception(int min, int max)
        {
            return Add(_exception, min, max);
        }

        public Pattern File()
        {
            return Add(_file);
        }
        public Pattern File(int min, int max)
        {
            return Add(_file, min, max);
        }

        public Pattern Identity()
        {
            return Add(_identity);
        }
        public Pattern Identity(int min, int max)
        {
            return Add(_identity, min, max);
        }

        public Pattern Location()
        {
            return Add(_location);
        }
        public Pattern Location(int min, int max)
        {
            return Add(_location, min, max);
        }

        public Pattern Logger()
        {
            return Add(_logger);
        }
        public Pattern Logger(int min, int max)
        {
            return Add(_logger, min, max);
        }

        public Pattern Level()
        {
            return Add(_level);
        }
        public Pattern Level(int min, int max)
        {
            return Add(_level, min, max);
        }

        public Pattern Line()
        {
            return Add(_line);
        }
        public Pattern Line(int min, int max)
        {
            return Add(_line, min, max);
        }

        public Pattern Message()
        {
            return Add(_message);
        }
        public Pattern Message(int min, int max)
        {
            return Add(_message, min, max);
        }

        public Pattern Method()
        {
            return Add(_method);
        }
        public Pattern Method(int min, int max)
        {
            return Add(_method, min, max);
        }

        public Pattern Newline()
        {
            return Add(_newline);
        }

        public Pattern Property()
        {
            return Add(_property);
        }
        public Pattern Property(int min, int max)
        {
            return Add(_property, min, max);
        }

        public Pattern TimeStamp()
        {
            return Add(_timestamp);
        }
        public Pattern TimeStamp(int min, int max)
        {
            return Add(_timestamp, min, max);
        }

        public Pattern Thread()
        {
            return Add(_thread);
        }
        public Pattern Thread(int min, int max)
        {
            return Add(_thread, min, max);
        }

        public Pattern Type()
        {
            return Add(_type);
        }
        public Pattern Type(int min, int max)
        {
            return Add(_type, min, max);
        }
        
        public Pattern UserName()
        {
            return Add(_username);
        }
        public Pattern UserName(int min, int max)
        {
            return Add(_username, min, max);
        }

        public string ToString()
        {
            return _builder.ToString();
        }
    }
}