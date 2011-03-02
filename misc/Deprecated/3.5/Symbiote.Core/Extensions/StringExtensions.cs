using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Log;

namespace Symbiote.Core.Extensions
{
    public static class StringExtenders
    {
        private static Dictionary<Type, Func<string, object>> _stringCoverters 
            = new Dictionary<Type, Func<string, object>>()
            {
                  {typeof(short), s =>
                  {
                      short v = 0;
                      short.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(int), s =>
                  {
                      var v = 0;
                      int.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(long), s =>
                  {
                      var v = 0l;
                      long.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(float), s =>
                  {
                      var v = 0f;
                      float.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(decimal), s =>
                  {
                      var v = 0m;
                      decimal.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(bool), s =>
                  {
                      var v = false;
                      bool.TryParse(s, out v);
                      return v;
                  }},
                  {typeof(Guid), s =>
                  {
                      var v = new Guid(s);
                      return v;
                  }},
                  {typeof(DateTime), s =>
                  {
                      var v = DateTime.MinValue;
                      DateTime.TryParse(s, out v);
                      return v;
                  }},     
            };

        public static string AsFormat(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static T ConvertTo<T>(this string value)
        {
            Func<string, object> converter = null;
            var type = typeof(T);
            if (!_stringCoverters.TryGetValue(type, out converter))
            {
                throw new Exception(
                    "Symbiote does not have a converter defined to convert from string to {0}".AsFormat(type.FullName));
            }
            return (T)converter(value);
        }

        public static string ToConsole(this string format, params object[] args)
        {
            var value = format.AsFormat(args);
            Console.WriteLine(value);
            return value;
        }

        public static void ToInfo<TLog>(this string message)
        {
            LogManager.Log<TLog>(LogLevel.Info, message);
        }

        public static void ToInfo<TLog>(this string message, params object[] args)
        {
            LogManager.Log<TLog>(LogLevel.Info, message, args);
        }

        public static void ToDebug<TLog>(this string message)
        {
            LogManager.Log<TLog>(LogLevel.Debug, message);
        }

        public static void ToDebug<TLog>(this string message, params object[] args)
        {
            LogManager.Log<TLog>(LogLevel.Debug, message, args);
        }

        public static void ToWarn<TLog>(this string message)
        {
            LogManager.Log<TLog>(LogLevel.Warn, message);
        }

        public static void ToWarn<TLog>(this string message, params object[] args)
        {
            LogManager.Log<TLog>(LogLevel.Warn, message, args);
        }

        public static void ToError<TLog>(this string message)
        {
            LogManager.Log<TLog>(LogLevel.Error, message);
        }

        public static void ToError<TLog>(this string message, params object[] args)
        {
            LogManager.Log<TLog>(LogLevel.Error, message, args);
        }

        public static void ToFatal<TLog>(this string message)
        {
            LogManager.Log<TLog>(LogLevel.Fatal, message);
        }

        public static void ToFatal<TLog>(this string message, params object[] args)
        {
            LogManager.Log<TLog>(LogLevel.Fatal, message, args);
        }

        
    }
}
