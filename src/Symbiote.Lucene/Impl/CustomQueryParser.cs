using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace Symbiote.Lucene.Impl
{
    public class CustomQueryParser : QueryParser
    {
        public CustomQueryParser(Version matchVersion, string f, Analyzer a) : base(matchVersion, f, a)
        {
            
        }

        protected override Query GetRangeQuery(string field, string part1, string part2, bool inclusive)
        {
            var fieldType = FieldType.FindNumericFieldType(part1) ?? FieldType.FindNumericFieldType(part2);
            if (fieldType != null)
            {
                return fieldType.ToRangeQuery(field, part1, part2, inclusive);
            }
            else
            {
                return base.GetRangeQuery(field, part1, part2, inclusive);
            }
        }
    }

    public class TypedField
    {
        protected readonly string _regexPattern = @"^(\\w+)(<(\\w+)>)?$";
        protected string FieldName { get; set; }
    }

    public class DateTimeFieldType : FieldType
    {
        public override NumericField ToField(string name, object value, Field.Store store)
        {
            return new NumericField(name, 8, store, true).SetLongValue(ToDateTime(value, DateTime.MinValue));
        }

        public override Query ToRangeQuery(string name, string min, string max, bool inclusive)
        {
            return NumericRangeQuery.NewLongRange(name, 8, ToDateTime(min, DateTime.MinValue), ToDateTime(max, DateTime.MaxValue), inclusive, inclusive);
        }

        public long ToDateTime(object obj, DateTime ifInvalid)
        {
            DateTime d = ifInvalid;
            if (!DateTime.TryParse(obj.ToString(), out d))
                d = ifInvalid;
            return d.Ticks;
        }

        public override bool IsValid(object value)
        {
            DateTime d;
            return DateTime.TryParse(value.ToString(), out d);
        }
    }

    public class DoubleFieldType : FieldType
    {
        public override NumericField ToField(string name, object value, Field.Store store)
        {
            return new NumericField(name, 8, store, true).SetDoubleValue(ToDouble(value, 0));
        }

        public override Query ToRangeQuery(string name, string min, string max, bool inclusive)
        {
            return NumericRangeQuery.NewDoubleRange(name, 9, ToDouble(min, 0), ToDouble(max, Double.MaxValue), inclusive, inclusive);
        }

        public double ToDouble(object obj, double ifInvalid)
        {
            double d = 0;
            if (!double.TryParse(obj.ToString(), out d))
                d = ifInvalid;
            return d;
        }

        public override bool IsValid(object value)
        {
            Double d;
            return Double.TryParse(value.ToString(), out d);
        }
    }

    public class FloatFieldType : FieldType
    {
        public override NumericField ToField(string name, object value, Field.Store store)
        {
            return new NumericField(name, 4, store, true).SetFloatValue(ToFloat(value, 0));   
        }

        public override Query ToRangeQuery(string name, string min, string max, bool inclusive)
        {
            return NumericRangeQuery.NewFloatRange(name, ToFloat(min, 0), ToFloat(max, float.MaxValue), inclusive, inclusive);
        }

        public float ToFloat(object obj, float ifInvalid)
        {
            float f = 0;
            if (!float.TryParse(obj.ToString(), out f))
                f = ifInvalid;
            return f;
        }

        public override bool IsValid(object value)
        {
            float f;
            return float.TryParse(value.ToString(), out f);
        }
    }

    public class IntFieldType : FieldType
    {
        public override NumericField ToField(string name, object value, Field.Store store)
        {
            return new NumericField(name, 4, store, true).SetIntValue(ToInt(value, 0));   
        }

        public override Query ToRangeQuery(string name, string min, string max, bool inclusive)
        {
            return NumericRangeQuery.NewIntRange(name, ToInt(min, 0), ToInt(max, int.MaxValue), inclusive, inclusive);
        }

        public int ToInt(object obj, int ifInvalid)
        {
            int i = 0;
            if (!int.TryParse(obj.ToString(), out i))
                i = ifInvalid;
            return i;
        }

        public override bool IsValid(object value)
        {
            int i;
            return int.TryParse(value.ToString(), out i);
        }
    }

    public class LongFieldType : FieldType
    {
        public override NumericField ToField(string name, object value, Field.Store store)
        {
            return new NumericField(name, 8, store, true).SetLongValue(ToLong(value, 0));
        }

        public override Query ToRangeQuery(string name, string min, string max, bool inclusive)
        {
            return NumericRangeQuery.NewLongRange(name, ToLong(min, 0), ToLong(max, long.MaxValue), inclusive, inclusive);
        }

        public long ToLong(object obj, long ifInvalid)
        {
            long l = 0;
            if (!long.TryParse(obj.ToString(), out l))
                l = ifInvalid;
            return l;
        }

        public override bool IsValid(object value)
        {
            long l;
            return long.TryParse(value.ToString(), out l);
        }
    }

    public abstract class FieldType
    {
        public abstract NumericField ToField(string name, object value, Field.Store store);
        public abstract Query ToRangeQuery(string name, string min, string max, bool inclusive);

        public abstract bool IsValid(object value);

        public static FieldType FindNumericFieldType(object value)
        {
            return AllFieldTypes().FirstOrDefault(x => x.IsValid(value));
        }

        protected static IEnumerable<FieldType> AllFieldTypes()
        {
            yield return new IntFieldType();
            yield return new LongFieldType();
            yield return new DateTimeFieldType();
            yield return new FloatFieldType();
            yield return new DoubleFieldType();
        }
    }

    
}
