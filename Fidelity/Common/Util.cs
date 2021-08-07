using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Fidelity.Common
{
    public class Util
    {
        private static string _baseUrl;
        private static string _AppKey;

        #region URL BASE

        public static string BaseUrl { get { return _baseUrl ?? GetBaseUrl(); } }

        public static string GetBaseUrl()
        {
            try
            {
                _baseUrl = "";

                return _baseUrl;
            }
            catch (Exception)
            {
                return "";
            }
        }


        #endregion

        /// <summary>
        /// Serializer customizado para ignorar listas vazias e não mandar no Json para a Service Layer
        /// </summary>
        public class IgnoreEmptyEnumerablesResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member,
                    MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                bool isDefaultValueIgnored =
                    ((property.DefaultValueHandling ?? DefaultValueHandling.Ignore)
                        & DefaultValueHandling.Ignore) != 0;
                if (isDefaultValueIgnored
                        && !typeof(string).IsAssignableFrom(property.PropertyType)
                        && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    Predicate<object> newShouldSerialize = obj =>
                    {
                        var collection = property.ValueProvider.GetValue(obj) as ICollection;
                        return collection == null || collection.Count != 0;
                    };
                    Predicate<object> oldShouldSerialize = property.ShouldSerialize;
                    property.ShouldSerialize = oldShouldSerialize != null
                        ? o => oldShouldSerialize(o) && newShouldSerialize(o)
                        : newShouldSerialize;
                }
                return property;
            }
        }
    }
}