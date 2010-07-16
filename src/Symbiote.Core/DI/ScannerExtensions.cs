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

        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            return type.Namespace.StartsWith(nameSpace);
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

        public static Type FindFirstInterfaceThatCloses(this Type pluggedType, Type templateType)
        {
            return pluggedType.FindInterfacesThatClose(templateType).FirstOrDefault();
        }

        public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            return rawFindInterfacesThatCloses(pluggedType, templateType).Distinct();
        }

        private static IEnumerable<Type> rawFindInterfacesThatCloses(Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) yield break;

            if (templateType.IsInterface)
            {
                foreach (var interfaceType in pluggedType.GetInterfaces().Where(type => type.IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedType.BaseType.IsGenericType && (pluggedType.BaseType.GetGenericTypeDefinition() == templateType))
            {
                yield return pluggedType.BaseType;
            }

            if (pluggedType.BaseType == typeof(object)) yield break;

            foreach (var interfaceType in rawFindInterfacesThatCloses(pluggedType.BaseType, templateType))
            {
                yield return interfaceType;
            }
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetInnerTypeFromNullable(this Type nullableType)
        {
            return nullableType.GetGenericArguments()[0];
        }

        public static bool CanBeCreated(this Type type)
        {
            return type.IsConcrete() && HasConstructors(type);
        }

        public static IEnumerable<Type> AllInterfaces(this Type type)
        {
            foreach (Type @interface in type.GetInterfaces())
            {
                yield return @interface;
            }
        }

        private static bool HasConstructors(Type pluggedType)
        {
            return GetGreediestConstructor(pluggedType) != null;
        }

        private static ConstructorInfo GetGreediestConstructor(Type pluggedType)
        {
            ConstructorInfo returnValue = null;

            foreach (ConstructorInfo constructor in pluggedType.GetConstructors())
            {
                if (returnValue == null)
                {
                    returnValue = constructor;
                }
                else if (constructor.GetParameters().Length > returnValue.GetParameters().Length)
                {
                    returnValue = constructor;
                }
            }

            return returnValue;
        }

        public static bool CanBeCastTo(this Type pluggedType, Type pluginType)
        {
            if (pluggedType == null) return false;

            if (pluggedType.IsInterface || pluggedType.IsAbstract)
            {
                return false;
            }

            if (pluginType.IsOpenGeneric())
            {
                return CanBeCast(pluginType, pluggedType);
            }

            if (IsOpenGeneric(pluggedType))
            {
                return false;
            }


            return pluginType.IsAssignableFrom(pluggedType);
        }

        private static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            try
            {
                return CheckGenericType(pluggedType, pluginType);
            }
            catch (Exception e)
            {
                string message =
                    string.Format("Could not Determine Whether Type '{0}' plugs into Type '{1}'",
                                  pluginType.Name,
                                  pluggedType.Name);
                throw new Exception(message, e);
            }
        }

        private static bool CheckGenericType(Type pluggedType, Type pluginType)
        {
            if (pluginType.IsAssignableFrom(pluggedType)) return true;


            // check interfaces
            foreach (Type type in pluggedType.GetInterfaces())
            {
                if (!type.IsGenericType)
                {
                    continue;
                }

                if (type.GetGenericTypeDefinition().Equals(pluginType))
                {
                    return true;
                }
            }

            if (pluggedType.BaseType.IsGenericType)
            {
                Type baseType = pluggedType.BaseType.GetGenericTypeDefinition();

                if (baseType.Equals(pluginType))
                {
                    return true;
                }
                else
                {
                    return CanBeCast(pluginType, baseType);
                }
            }

            return false;
        }

        public static bool IsString(this Type type)
        {
            return type.Equals(typeof(string));
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive && !IsString(type) && type != typeof(IntPtr);
        }

        public static bool IsSimple(this Type type)
        {
            return type.IsPrimitive || IsString(type) || type.IsEnum;
        }

        public static bool IsChild(this Type type)
        {
            return IsPrimitiveArray(type) || (!type.IsArray && !IsSimple(type));
        }

        public static bool IsChildArray(this Type type)
        {
            return type.IsArray && !IsSimple(type.GetElementType());
        }

        public static bool IsPrimitiveArray(this Type type)
        {
            return type.IsArray && IsSimple(type.GetElementType());
        }

        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        public static bool IsAutoFillable(this Type type)
        {
            return IsChild(type) || IsChildArray(type);
        }
    }
}
