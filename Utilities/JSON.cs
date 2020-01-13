using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            value = value.Replace("'", "\"");
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(value, options);
            }
            catch(Exception ex)
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
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                return System.Text.Json.JsonSerializer.Serialize(obj, options);
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
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                switch (format)
                {
                    case 0:
                    default:
                        return System.Text.Json.JsonSerializer.Serialize(obj, options);
                    case 1:
                        return System.Text.Json.JsonSerializer.Serialize(obj, options);                        
                }
            }
            catch(Exception ex)
            {
                Console.Write("Error serializing object: " + ex.Message);
                return null;
            }
        }
    }
    public class DoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
            {
                string value = reader.GetString();
                try
                {
                    return Double.Parse(value);
                }
                catch (FormatException)
                {
                    throw new JsonException();
                }
            }
            else
            {
                return reader.GetDouble();
            }
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class BooleanConverter : JsonConverter<Boolean>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == System.Text.Json.JsonTokenType.String)
            {
                string value = reader.GetString();
                try
                {
                    return Boolean.Parse(value);
                }
                catch (FormatException)
                {
                    throw new JsonException();
                }
            }
            else
            {
                return reader.GetBoolean();
            }
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
