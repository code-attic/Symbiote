using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core
{
    public class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 value)
        {
            return new Tuple<T1>() { Value1 = value };
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 value, T2 value2)
        {
            return new Tuple<T1, T2>()
            {
                Value1 = value,
                Value2 = value2
            };
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 value, T2 value2, T3 value3)
        {
            return new Tuple<T1, T2, T3>()
            {
                Value1 = value,
                Value2 = value2,
                Value3 = value3
            };
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 value, T2 value2, T3 value3, T4 value4)
        {
            return new Tuple<T1, T2, T3, T4>()
            {
                Value1 = value,
                Value2 = value2,
                Value3 = value3,
                Value4 = value4
            };
        }

        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 value, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            return new Tuple<T1, T2, T3, T4, T5>()
            {
                Value1 = value,
                Value2 = value2,
                Value3 = value3,
                Value4 = value4,
                Value5 = value5
            };
        }

        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 value, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6>()
            {
                Value1 = value,
                Value2 = value2,
                Value3 = value3,
                Value4 = value4,
                Value5 = value5,
                Value6 = value6
            };
        }
    }

    [Serializable]
    public class Tuple<T1>
    {
        public T1 Value1 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1>;
            if (compare == null)
                return false;
            return compare.Value1.Equals(Value1);
        }

        public static bool operator ==(Tuple<T1> value1, Tuple<T1> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1> value1, Tuple<T1> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class Tuple<T1, T2>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1, T2>;
            if (compare == null)
                return false;
            return
                compare.Value1.Equals(Value1) &&
                compare.Value2.Equals(Value2);
        }

        public static bool operator ==(Tuple<T1, T2> value1, Tuple<T1, T2> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1, T2> value1, Tuple<T1, T2> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                result = (result * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1, T2, T3>;
            if (compare == null)
                return false;
            return
                compare.Value1.Equals(Value1) &&
                compare.Value2.Equals(Value2) &&
                compare.Value3.Equals(Value3);
        }

        public static bool operator ==(Tuple<T1, T2, T3> value1, Tuple<T1, T2, T3> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1, T2, T3> value1, Tuple<T1, T2, T3> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                result = (result * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                result = (result * 397) ^ (Value3 != null ? Value3.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }
        public T4 Value4 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1, T2, T3, T4>;
            if (compare == null)
                return false;
            return
                compare.Value1.Equals(Value1) &&
                compare.Value2.Equals(Value2) &&
                compare.Value3.Equals(Value3) &&
                compare.Value4.Equals(Value4);
        }

        public static bool operator ==(Tuple<T1, T2, T3, T4> value1, Tuple<T1, T2, T3, T4> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1, T2, T3, T4> value1, Tuple<T1, T2, T3, T4> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                result = (result * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                result = (result * 397) ^ (Value3 != null ? Value3.GetHashCode() : 0);
                result = (result * 397) ^ (Value4 != null ? Value4.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }
        public T4 Value4 { get; set; }
        public T5 Value5 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1, T2, T3, T4, T5>;
            if (compare == null)
                return false;
            return
                compare.Value1.Equals(Value1) &&
                compare.Value2.Equals(Value2) &&
                compare.Value3.Equals(Value3) &&
                compare.Value4.Equals(Value4) &&
                compare.Value5.Equals(Value5);
        }

        public static bool operator ==(Tuple<T1, T2, T3, T4, T5> value1, Tuple<T1, T2, T3, T4, T5> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1, T2, T3, T4, T5> value1, Tuple<T1, T2, T3, T4, T5> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                result = (result * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                result = (result * 397) ^ (Value3 != null ? Value3.GetHashCode() : 0);
                result = (result * 397) ^ (Value4 != null ? Value4.GetHashCode() : 0);
                result = (result * 397) ^ (Value5 != null ? Value5.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5, T6>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }
        public T4 Value4 { get; set; }
        public T5 Value5 { get; set; }
        public T6 Value6 { get; set; }

        public override bool Equals(object obj)
        {
            var compare = obj as Tuple<T1, T2, T3, T4, T5, T6>;
            if (compare == null)
                return false;
            return
                compare.Value1.Equals(Value1) &&
                compare.Value2.Equals(Value2) &&
                compare.Value3.Equals(Value3) &&
                compare.Value4.Equals(Value4) &&
                compare.Value5.Equals(Value5) &&
                compare.Value6.Equals(Value6);
        }

        public static bool operator ==(Tuple<T1, T2, T3, T4, T5, T6> value1, Tuple<T1, T2, T3, T4, T5, T6> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;
            if (ReferenceEquals(value1, null))
                return false;
            return value1.Equals(value2);
        }

        public static bool operator !=(Tuple<T1, T2, T3, T4, T5, T6> value1, Tuple<T1, T2, T3, T4, T5, T6> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;
            if (ReferenceEquals(value1, null))
                return true;
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Value1 != null ? Value1.GetHashCode() : 0);
                result = (result * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                result = (result * 397) ^ (Value3 != null ? Value3.GetHashCode() : 0);
                result = (result * 397) ^ (Value4 != null ? Value4.GetHashCode() : 0);
                result = (result * 397) ^ (Value5 != null ? Value5.GetHashCode() : 0);
                result = (result * 397) ^ (Value6 != null ? Value6.GetHashCode() : 0);
                return result;
            }
        }
    }
}
