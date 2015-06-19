using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace VK_Api_Tests.VK
{
    public abstract class VkObject
    {
        private delegate void InitializerDelegate(VkObject obj, JToken value);

        private static readonly Dictionary<Type, InitializerDelegate> CachedInitializers = new Dictionary<Type, InitializerDelegate>();


        protected VkObject(JToken json)
        {
            var type = this.GetType();

            InitializerDelegate initMethod;
            if (!CachedInitializers.TryGetValue(type, out initMethod))
                CachedInitializers.Add(type, initMethod = CreateInitializer(type));

            initMethod(this, json);
        }

        private static string CamelCaseToSnakeCase(string text)
        {
            return Regex.Replace(text, @"(\p{Ll})(\p{Lu})", @"$1_$2").ToLower();
        }
        private static InitializerDelegate CreateInitializer(Type type)
        {
            var objParam = Expression.Parameter(typeof(VkObject));
            var jsonParam = Expression.Parameter(typeof(JToken));
            var castedInstance = Expression.Variable(type);

            var blockBody = new Expression[] { Expression.Assign(castedInstance, Expression.Convert(objParam, type)) }
                .Concat(type.GetProperties()
                    .Select(property =>
                        Expression.Assign(
                            Expression.MakeMemberAccess(castedInstance, property),
                            typeof(VkObject).IsAssignableFrom(property.PropertyType)
                                ? Expression.New(property.PropertyType.GetConstructor(new[] { typeof(JToken) }),
                                    Expression.Call(jsonParam,
                                        "Value",
                                        new[] { typeof(JToken) },
                                        Expression.Constant(CamelCaseToSnakeCase(property.Name),
                                            typeof(string)))) as Expression
                                : Expression.Call(jsonParam,
                                    "Value",
                                    new[] { property.PropertyType },
                                    Expression.Constant(CamelCaseToSnakeCase(property.Name),
                                        typeof(string))) as Expression
                            )));

            return Expression.Lambda<InitializerDelegate>(
                Expression.Block(
                    new[] { castedInstance },
                    blockBody),
                objParam, jsonParam)
                .Compile();
        }
    }
}
