using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core
{
    public class DelimitedBuilder
    {
        #region Fields

        private StringBuilder _builder;
        private string _delimiter = ", ";

        #endregion

        #region Public Methods

        public void Append(object value)
        {
            _builder.Append(value.ToString());
            _builder.Append(_delimiter);
        }

        public void Append(object value, bool delimiter)
        {
            _builder.Append(value.ToString());
            if (delimiter)
                _builder.Append(_delimiter);
        }

        public void Append(object value, object prefix, object suffix)
        {
            _builder.AppendFormat("{0}{1}{2}", prefix.ToString(), value.ToString(), suffix.ToString());
            _builder.Append(_delimiter);
        }

        public void Append(object value, object prefix, object suffix, bool delimiter)
        {
            _builder.AppendFormat("{0}{1}{2}", prefix.ToString(), value.ToString(), suffix.ToString());
            if (delimiter)
                _builder.Append(_delimiter);
        }

        public void Append(string value)
        {
            _builder.Append(value);
            _builder.Append(_delimiter);
        }

        public void Append(string value, bool delimiter)
        {
            _builder.Append(value);
            if (delimiter)
                _builder.Append(_delimiter);
        }

        public void Append(string value, string prefix, string suffix)
        {
            _builder.AppendFormat("{0}{1}{2}", prefix.ToString(), value.ToString(), suffix.ToString());
            _builder.Append(_delimiter);
        }

        public void Append(string value, string prefix, string suffix, bool delimiter)
        {
            _builder.AppendFormat("{0}{1}{2}", prefix.ToString(), value.ToString(), suffix.ToString());
            if (delimiter)
                _builder.Append(_delimiter);
        }

        public void AppendFormat(string format, params object[] values)
        {
            AppendFormat(format, true, values);
        }

        public void AppendFormat(string format, bool delimiter, params object[] values)
        {
            _builder.AppendFormat(format, values);
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

        #endregion

    }
}
