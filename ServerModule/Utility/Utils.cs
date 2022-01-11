using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using DebugAndTrace;
using ServerModule.Responses;

namespace ServerModule.Utility
{
    public enum Method
    {
        Get,
        Post,
        Put,
        Delete,
        Patch,
        Head,
        Error
    }

    public enum Char
    {
        WhiteSpace,
        NewLine,
        QuestionMark,
        Slash,
        Colon,
        DoubleQuote,
        OpenBracket,
        Minus
    }

    public static class Utils
    {
        private static readonly IPrinter Log = Logger.GetPrinter(Printer.Debug);
        private static readonly Dictionary<Char, char> Characters = new()
        {
            { Char.WhiteSpace, ' ' },
            { Char.NewLine, '\n' },
            { Char.QuestionMark, '?' },
            { Char.Slash, '/' },
            { Char.Colon, ':' },
            { Char.DoubleQuote, '\"' },
            { Char.OpenBracket, '[' },
            { Char.Minus, '-' }
        };

        /// <summary>
        /// Get the character with the given Enum key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns the corresponding character</returns>
        public static char GetChar(Char key)
        {
            // See: https://www.dotnetperls.com/static-dictionary
            Characters.TryGetValue(key, out char value);
            return value;
        }

        public static Method GetMethod(string method)
        {
            method = method.ToLower();
            return method switch
            {
                "get" => Method.Get,
                "post" => Method.Post,
                "put" => Method.Put,
                "delete" => Method.Delete,
                "patch" => Method.Patch,
                "head" => Method.Head,
                _ => Method.Error
            };
        }

        public static string GetResponseStatusText(int status)
        {
            // if using integer instead of enum Status, needs to check if Status contains the integer
            // See: https://stackoverflow.com/a/12291985
            return !Enum.IsDefined(typeof(Status), status) ? "No status text" : ((Status)status).ToString().ToUpper();
        }

        // Cast dictionary<string,object> to class
        // See: https://stackoverflow.com/a/28323995
        public static object GetObject(this Dictionary<string, object> dict, Type type)
        {
            try
            {
                var obj = Activator.CreateInstance(type);
                foreach (var kv in dict)
                {
                    PropertyInfo prop = type.GetProperty(kv.Key);
                    if (prop == null) continue;
                    Type propType = prop.PropertyType;
                    object value = kv.Value;
                    if (value is Dictionary<string, object> dictionary)
                    {
                        value = GetObject(dictionary, prop.PropertyType);
                    }
                    if (value is JsonElement rawValue)
                    {
                        switch (rawValue.ValueKind)
                        {
                            case JsonValueKind.String:
                                value = value.ToString();
                                break;
                            case JsonValueKind.Number:
                                value = Convert.ToInt32(value);
                                break;
                        }
                    }
                    if (value != null && propType == value.GetType())
                    {
                        prop.SetValue(obj, value, null);
                    }
                }
                return obj;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                throw;
            }
        }

        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            return (T)GetObject(dict, typeof(T));
        }

        /// <summary>
        /// Add all property names with the corresponding value into a string.
        /// </summary>
        /// <param name="myObj"></param>
        /// <returns>Returns the string with the values or "" if the object is empty/null</returns>
        public static string GetProperties(this object myObj)
        {
            // Using reflection
            // See: https://stackoverflow.com/a/19823887
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyInfo prop in myObj.GetType().GetProperties())
            {
                stringBuilder.AppendLine(prop.Name + ": " + prop.GetValue(myObj, null));
            }
            return stringBuilder.ToString();
        }
    }
}