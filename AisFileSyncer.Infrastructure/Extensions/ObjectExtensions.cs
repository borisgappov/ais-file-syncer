using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AisFileSyncer.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string message = "")
        {
            if (obj == null)
            {
                if (message.IsEmpty())
                {
                    throw new ArgumentNullException(typeof(T).Name);
                }
                else
                {
                    throw new Exception(message);
                }
            }
            else
            {
                return obj;
            }
        }

        static public string ToJSON(this object Object)
        {
            return JsonConvert.SerializeObject(Object, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });
        }

        static public T FromJSON<T>(this string JSON, T DefaultValue, JsonSerializerSettings settings = null)
        {
            try
            {
                return string.IsNullOrEmpty(JSON)
                    ? DefaultValue
                    : (T)JsonConvert.DeserializeObject(JSON, typeof(T), settings);
            }
            catch
            {
                return DefaultValue;
            }
        }

        static public dynamic FromJSON(this string JSON)
        {
            try
            {
                return string.IsNullOrEmpty(JSON) ? null : JObject.Parse(JSON);
            }
            catch
            {
                return null;
            }
        }

        static public T FromJSON<T>(this string json)
        {
            try
            {
                return string.IsNullOrEmpty(json)
                    ? default(T)
                    : (T)JsonConvert.DeserializeObject(json, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        static public T Clone<T>(this T source)
        {
            return source.ToJSON().FromJSON<T>();
        }
    }

}
