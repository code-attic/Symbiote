using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Core
{
    public class DelimitedBuilder
    {
        #region Fields

        private StringBuilder _builder;
        private string _delimiter = ", ";
        private string _prefix = "";
        private string _suffix = "";

        #endregion

        #region Public Methods

        public void Append(object value)
        {
            Append(value, true);
        }

        public void Append(object value, bool delimiter)
        {
            Append(value, "", "", delimiter);
        }

        public void Append(object value, object prefix, object suffix)
        {
            Append(value, prefix, suffix, true);
        }

        public void Append(object value, object prefix, object suffix, bool delimiter)
        {
            Append(value.ToString(), prefix.ToString(), suffix.ToString(), delimiter);
        }

        public void Append(string value)
        {
            Append(value, true);
        }

        public void Append(string value, bool delimiter)
        {
            Append(value, "", "", delimiter);
        }

        public void Append(string value, string prefix, string suffix)
        {
            Append(value, prefix, suffix, true);
        }

        public void Append(string value, string prefix, string suffix, bool delimiter)
        {
            AppendFormat("{0}{1}{2}", delimiter, prefix, suffix, value);
        }

        public void AppendFormat(string format, params object[] values)
        {
            AppendFormat(format, true, values);
        }

        public void AppendFormat(string format, bool delimiter, params object[] values)
        {
            var value = format.AsFormat(values);
            _builder.AppendFormat("{0}{1}{2}", _prefix, value, _suffix);
            if (delimiter)
                _builder.Append(_delimiter);
        }

        public void Clear()
        {
            _builder = new StringBuilder();
        }

        public static string Construct(IEnumerable<string> source, string delimiter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string s in source)
                builder.AppendFormat("{0}{1}", s, delimiter);

            return TrimDelimiter(delimiter, builder.ToString());
        }

        public static string Construct(string[] source, string delimiter, string prefix, string suffix)
        {
            return Construct(new List<string>(source), delimiter, prefix, suffix);
        }

        public static string Construct(List<string> source, string delimiter, string prefix, string suffix)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string s in source)
                builder.AppendFormat("{0}{1}{2}{3}", prefix, s, suffix, delimiter);

            return TrimDelimiter(delimiter, builder.ToString());
        }

        private static string TrimDelimiter(string delimiter, string source)
        {
            return !source.EndsWith(delimiter) ? source : source.Substring(0,
                (source.Length - delimiter.Length) <= 0 ? source.Length : source.Length - delimiter.Length);
        }
        #endregion

        #region Overloaded Methods

        public override string ToString()
        {
            return TrimDelimiter(_delimiter, _builder.ToString());
        }



        #endregion

        #region Constructors

        public DelimitedBuilder()
        {
            _builder = new StringBuilder();
        }

        public DelimitedBuilder(string delimiter)
        {
            _builder = new StringBuilder();
            _delimiter = delimiter;
        }

        public DelimitedBuilder(string delimiter, string prefix, string suffix)
        {
            _builder = new StringBuilder();
            _delimiter = delimiter;
            _prefix = prefix;
            _suffix = suffix;
        }

        #endregion

    }
}
