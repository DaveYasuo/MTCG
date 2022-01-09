﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using ServerModule.SimpleLogic.Responses;

namespace ServerModule.Utility
{
    public enum Method
    {
        Get, Post, Put, Delete, Patch, Error
    }
    public enum Char
    {
        WhiteSpace, NewLine, QuestionMark, Slash, Colon, DoubleQuote, OpenBracket, Minus
    }
    public static class Utils
    {
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
            if (Characters.TryGetValue(key, out char value))
            {
                return value;
            }
            throw new NotImplementedException();
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
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            return (T)GetObject(dict, typeof(T));
        }
    }

}