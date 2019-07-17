using System;
using Newtonsoft.Json;

namespace Utilities
{
    /// <summary>
    /// JSON utility wrapper class
    /// </summary>
    public class JSON
    {

        /// <summary>
        /// Deserializer wrapper function, deserializes object from json string using Json.NET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Json string</param>
        /// <param name="obj">return Object</param>
        /// <returns>Object T</returns>
        public static T Deserialize<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch(JsonReaderException ex)
            {
                Console.WriteLine("Error deserializing object: " + ex.Message);
                return default(T);
            }
            catch(JsonException ex)
            {
                Console.WriteLine("Error deserializing object: " + ex.Message);
                return default(T);
            }
        }

        /// <summary>
        /// Serializer wrapper function, serializes object to json string using Json.NET
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>string</returns>
        public static string Serialize(Object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.None);
            }
            catch(Exception ex)
            {
                Console.Write("Error serializing object: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Serializer wrapper function, serializes object to json using Json.Net
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="format">Format object: 0 default/None, 1: Indented</param>
        /// <returns>json formatted string</returns>
        public static string Serialize(Object obj, int format)
        {
            try
            {
                switch (format)
                {
                    case 0:
                    default:
                        return JsonConvert.SerializeObject(obj, Formatting.None);
                    case 1:
                        return JsonConvert.SerializeObject(obj, Formatting.Indented);                        
                }
            }
            catch(Exception ex)
            {
                Console.Write("Error serializing object: " + ex.Message);
                return null;
            }
        }
    }
}
