using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public static class ScannerExtensions
    {
        public static bool Closes(this Type type, Type openType)
        {
            if(!openType.IsGenericType && !openType.IsGenericTypeDefinition)
                return false;

            bool closes;

            if(openType.IsInterface)
            {
                closes = type.ImplementsInterfaceTemplate(openType) && type.IsGenericTypeDefinition == false;
            }
            else
            {
                closes = type.BaseType == openType;
            }
            if (closes) return true;
            return type.BaseType == null ? false : type.BaseType.Closes(openType);
        }

        public static bool IsOpenGeneric(this Type type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static bool IsConcreteAndAssignableTo(this Type pluggedType, Type pluginType)
        {
            return pluggedType.IsConcrete() && pluginType.IsAssignableFrom(pluggedType);
        }

        public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) return false;

            foreach (Type interfaceType in pluggedType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }
    }
}
