using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Restfully.Tests
{
    public static class DLinqSerializer
    {
        public static XElement SerializeQuery(this IQueryable query)
        {
            DLinqSerializationTypeResolver resolver = new DLinqSerializationTypeResolver(null);
            ExpressionSerializer serializer = new ExpressionSerializer(resolver) { Converters = { new DLinqCustomExpressionXmlConverter(null, resolver) } };
            return serializer.Serialize(query.Expression);
        }

        public static IQueryable DeserializeQuery(this DataContext dc, XElement rootXml)
        {
            DLinqSerializationTypeResolver resolver = new DLinqSerializationTypeResolver(dc);
            DLinqCustomExpressionXmlConverter customConverter = new DLinqCustomExpressionXmlConverter(dc, resolver);
            ExpressionSerializer serializer = new ExpressionSerializer(resolver) { Converters = { customConverter } };
            Expression queryExpr = serializer.Deserialize(rootXml);
            // Query kind is populated by the ResolveXmlFromExpression method
            if (customConverter.QueryKind == null)
                throw new Exception(string.Format("CAnnot deserialize into DLinq query for datacontext {0} - no Table found", dc));
            return customConverter.QueryKind.Provider.CreateQuery(queryExpr);
        }

        private class DLinqSerializationTypeResolver : ExpressionSerializationTypeResolver
        {
            private DataContext dc;

            public DLinqSerializationTypeResolver(DataContext dc)
            {
                this.dc = dc;
            }

            protected override Type ResolveTypeFromString(string typeString)
            {
                HashSet<Type> dataContextTableTypes = new HashSet<Type>(dc.Mapping.GetTables().Select(mt => mt.RowType.Type));
                if (typeString.Contains('`'))
                    return null;
                if (typeString.Contains(','))
                    typeString.Substring(0, typeString.IndexOf(','));
                
                foreach (Type tableType in dataContextTableTypes)
                {
                    if (typeString.EndsWith(tableType.Name))
                        return tableType;
                    if (typeString.EndsWith(tableType.Name + "[]"))
                        return typeof(EntitySet<>).MakeGenericType(tableType);
                }
                return null;
            }

        }

        private class DLinqCustomExpressionXmlConverter : CustomExpressionXmlConverter
        {
            private DataContext dc;
            private ExpressionSerializationTypeResolver resolver;
            public IQueryable QueryKind { get; private set; }
            
            public DLinqCustomExpressionXmlConverter(DataContext dc, ExpressionSerializationTypeResolver resolver)
            {
                this.dc = dc;
                this.resolver = resolver;
            }

            public override Expression Deserialize(XElement expressionXml)
            {
                if (expressionXml.Name.LocalName == "Table")
                {
                    Type type = resolver.GetType(expressionXml.Attribute("Type").Value);
                    ITable table = dc.GetTable(type);
                    // REturning a random IQueryable of the right kind so that we can re-create the IQueryable
                    // instance at the end of this method...
                    QueryKind = table;
                    return Expression.Constant(table);
                }
                return null;
            }

            public override XElement Serialize(Expression expression)
            {
                if (typeof(IQueryService).IsAssignableFrom(expression.Type))
                {
                    return new XElement("Table",
                        new XAttribute("Type", expression.Type.GetGenericArguments()[0].FullName));
                }
                return null;
            }
        }
    }
}
