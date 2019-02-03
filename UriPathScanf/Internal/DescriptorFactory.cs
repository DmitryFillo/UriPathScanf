using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UriPathScanf.Internal
{
    internal static class DescriptorFactory
    {
        public static Func<IDictionary<string, string>, T> GetFactory<T>() where T : class, new()
        {
            var type = typeof(T);
            var ctorEx = Expression.New(type);
            var dicParamEx = Expression.Parameter(typeof(IDictionary<string, string>), "d");

            var memberAssignments = type.GetTypeInfo().DeclaredProperties.Select(method =>
                Expression.Bind(method,
                    Expression.Call(GetPropertyValueMethod(method.PropertyType), dicParamEx, Expression.Constant(method.Name))));

            return Expression
                .Lambda<Func<IDictionary<string, string>, T>>(
                    Expression.MemberInit(ctorEx, memberAssignments),
                    dicParamEx).Compile();
        }

        private static T GetPropertyValue<T>(IDictionary<string, string> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val)) return default(T);

            try
            {
                return (T) Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        private static MethodInfo GetPropertyValueMethod(Type propType) =>
            typeof(DescriptorFactory)
                .GetTypeInfo()
                .GetMethod(nameof(GetPropertyValue), BindingFlags.NonPublic)
                .MakeGenericMethod(propType);
    }
}
