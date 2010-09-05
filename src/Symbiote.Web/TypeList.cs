using System;
using System.Collections.Generic;

namespace Symbiote.Web
{
    public class TypeList : List<Type>
    {
        public TypeList AddType<T>()
        {
            this.Add(typeof(T));
            return this;
        }

        public TypeList AddType(Type type)
        {
            this.Add(type);
            return this;
        }
    }
}