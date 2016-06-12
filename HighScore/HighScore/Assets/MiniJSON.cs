/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MiniJSON {
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json {

        private enum DataType
        {
            Int,
            Float,
            Boolean,
            String,
            Array,
            Object,
            Unknown
        }

        private static Json.DataType checkObjectType(object obj)
        {
            if (obj is string)
            {
                return Json.DataType.String;
            }
            if (obj is int)
            {
                return Json.DataType.Int;
            }
            if (obj is float)
            {
                return Json.DataType.Float;
            }
            if (obj is Dictionary<string, object>)
            {
                return Json.DataType.Object;
            }
            if (obj is List<object>)
            {
                return Json.DataType.Array;
            }
            if (obj is bool)
            {
                return Json.DataType.Boolean;
            }
            return Json.DataType.Unknown;
        }

        public static object ToObject(object output, object input)
        {
            Thread.Sleep(0);
            if (input == null)
            {
                output = null;
                return output;
            }
            string text = "DestType:" + output.GetType().ToString() + " >> ";
            FieldInfo[] fields = output.GetType().GetFields();
            Dictionary<string, object> dictionary = input as Dictionary<string, object>;
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                if (dictionary.ContainsKey(fieldInfo.Name))
                {
                    if (dictionary[fieldInfo.Name] != null)
                    {
                        Json.DataType dataType = Json.checkObjectType(dictionary[fieldInfo.Name]);
                        if (dataType != Json.DataType.Unknown)
                        {
                            try
                            {
                                Type fieldType = fieldInfo.FieldType;
                                object obj = null;
                                bool flag = false;
                                switch (dataType)
                                {
                                    case Json.DataType.Int:
                                        {
                                            object obj2 = (int)dictionary[fieldInfo.Name];
                                            if (fieldType == typeof(string))
                                            {
                                                obj = obj2.ToString();
                                            }
                                            else
                                            {
                                                if (fieldType == typeof(bool))
                                                {
                                                    bool flag2 = (int)obj2 != 0;
                                                    obj = flag2;
                                                }
                                                else
                                                {
                                                    if (fieldType.IsArray)
                                                    {
                                                        obj = Activator.CreateInstance(fieldType, new object[]
												{
													1
												});
                                                        Type elementType = obj.GetType().GetElementType();
                                                        object value = Activator.CreateInstance(elementType);
                                                        int num = (int)obj2;
                                                        value = num;
                                                        ((Array)obj).SetValue(value, 0);
                                                    }
                                                    else
                                                    {
                                                        obj = obj2;
                                                    }
                                                }
                                            }
                                            flag = true;
                                            break;
                                        }
                                    case Json.DataType.Float:
                                        {
                                            object obj3 = (float)dictionary[fieldInfo.Name];
                                            if (fieldType == typeof(string))
                                            {
                                                obj = (obj3 as string);
                                            }
                                            else
                                            {
                                                if (fieldType == typeof(int))
                                                {
                                                    float num2 = (float)obj3;
                                                    obj = (int)num2;
                                                }
                                                else
                                                {
                                                    if (fieldType.IsArray)
                                                    {
                                                        obj = Activator.CreateInstance(fieldType, new object[]
												{
													1
												});
                                                        Type elementType2 = obj.GetType().GetElementType();
                                                        object value2 = Activator.CreateInstance(elementType2);
                                                        float num3 = (float)obj3;
                                                        value2 = num3;
                                                        ((Array)obj).SetValue(value2, 0);
                                                    }
                                                    else
                                                    {
                                                        obj = obj3;
                                                    }
                                                }
                                            }
                                            flag = true;
                                            break;
                                        }
                                    case Json.DataType.Boolean:
                                        if (fieldType == typeof(bool))
                                        {
                                            obj = (bool)dictionary[fieldInfo.Name];
                                            flag = true;
                                        }
                                        break;
                                    case Json.DataType.String:
                                        {
                                            string text2 = dictionary[fieldInfo.Name] as string;
                                            if (fieldType == typeof(string))
                                            {
                                                obj = text2;
                                            }
                                            else
                                            {
                                                if (fieldType == typeof(int))
                                                {
                                                    int num4 = 0;
                                                    if (int.TryParse(text2, out num4))
                                                    {
                                                        obj = num4;
                                                    }
                                                    else
                                                    {
                                                        obj = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (fieldType == typeof(float))
                                                    {
                                                        float num5 = 0f;
                                                        if (float.TryParse(text2, out num5))
                                                        {
                                                            obj = float.Parse(text2);
                                                        }
                                                        else
                                                        {
                                                            obj = 0f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (fieldType.IsEnum)
                                                        {
                                                            if (Enum.IsDefined(fieldType, text2.ToString()))
                                                            {
                                                                obj = Enum.Parse(fieldType, text2.ToString());
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (fieldType.IsArray)
                                                            {
                                                                obj = Activator.CreateInstance(fieldType, new object[]
														{
															1
														});
                                                                ((Array)obj).SetValue(text2, 0);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            flag = true;
                                            break;
                                        }
                                    case Json.DataType.Array:
                                        if (fieldType.IsArray)
                                        {
                                            List<object> list = dictionary[fieldInfo.Name] as List<object>;
                                            if (list != null && (list.Count != 1 || list[0] != null))
                                            {
                                                obj = Activator.CreateInstance(fieldType, new object[]
											{
												list.Count
											});
                                                Type elementType3 = obj.GetType().GetElementType();
                                                for (int j = 0; j < list.Count; j++)
                                                {
                                                    Thread.Sleep(0);
                                                    object obj4;
                                                    if (elementType3 == typeof(string))
                                                    {
                                                        obj4 = new string[1];
                                                    }
                                                    else
                                                    {
                                                        obj4 = Activator.CreateInstance(elementType3);
                                                    }
                                                    try
                                                    {
                                                        Json.DataType dataType2 = Json.checkObjectType(list[j]);
                                                        if (dataType2 == Json.DataType.String)
                                                        {
                                                            obj4 = (list[j] as string);
                                                        }
                                                        else
                                                        {
                                                            if (dataType2 == Json.DataType.Object)
                                                            {
                                                                Json.ToObject(obj4, list[j]);
                                                            }
                                                            else
                                                            {
                                                                if (dataType2 == Json.DataType.Int || dataType2 == Json.DataType.Float)
                                                                {
                                                                    if (elementType3 == typeof(float))
                                                                    {
                                                                        obj4 = (float)list[j];
                                                                    }
                                                                    else
                                                                    {
                                                                        obj4 = (int)list[j];
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    obj4 = list[j];
                                                                }
                                                            }
                                                        }
                                                        ((Array)obj).SetValue(obj4, j);
                                                    }
                                                    catch (Exception var_29_560)
                                                    {
                                                    }
                                                }
                                                flag = true;
                                            }
                                        }
                                        break;
                                    case Json.DataType.Object:
                                        {
                                            object output2 = Activator.CreateInstance(fieldType);
                                            obj = Json.ToObject(output2, dictionary[fieldInfo.Name]);
                                            flag = true;
                                            break;
                                        }
                                    default:
                                        obj = Activator.CreateInstance(fieldType);
                                        break;
                                }
                                if (flag)
                                {
                                    fieldInfo.SetValue(output, obj);
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                            }
                        }
                    }
                }
            }
            return output;
        }

        public static T ToObjectArray<T>(object input)
        {
            Type typeFromHandle = typeof(T);
            List<object> list = input as List<object>;
            object obj = null;
            if (list == null || (list.Count == 1 && list[0] == null))
            {
                return (T)((object)obj);
            }
            obj = Activator.CreateInstance(typeFromHandle, new object[]
			{
				list.Count
			});
            Type elementType = obj.GetType().GetElementType();
            for (int i = 0; i < list.Count; i++)
            {
                object obj2;
                if (elementType == typeof(string))
                {
                    obj2 = new string[1];
                }
                else
                {
                    obj2 = Activator.CreateInstance(elementType);
                }
                try
                {
                    Json.DataType dataType = Json.checkObjectType(list[i]);
                    if (dataType == Json.DataType.String)
                    {
                        obj2 = (list[i] as string);
                    }
                    else
                    {
                        if (dataType == Json.DataType.Object)
                        {
                            Json.ToObject(obj2, list[i]);
                        }
                        else
                        {
                            if (dataType == Json.DataType.Int || dataType == Json.DataType.Float)
                            {
                                if (elementType == typeof(float))
                                {
                                    obj2 = (float)list[i];
                                }
                                else
                                {
                                    obj2 = (int)list[i];
                                }
                            }
                            else
                            {
                                obj2 = list[i];
                            }
                        }
                    }
                    ((Array)obj).SetValue(obj2, i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Concat(new object[]
					{
						ex.Message,
						"\nArray Member mismatch: [",
						typeFromHandle.Name,
						"] ReqType is ",
						elementType,
						" But ",
						Json.checkObjectType(list[i]).ToString().ToString(),
						" data Comes"
					}));
                }
            }
            return (T)((object)obj);
        }

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json) {
            // save the string for debug information
            if (json == null) {
                return null;
            }

            return Parser.Parse(json);
        }

        sealed class Parser : IDisposable {
            const string WORD_BREAK = "{}[],:\"";

            public static bool IsWordBreak(char c) {
                return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }

            enum TOKEN {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            };

            StringReader json;

            Parser(string jsonString) {
                json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString) {
                using (var instance = new Parser(jsonString)) {
                    return instance.ParseValue();
                }
            }

            public void Dispose() {
                json.Dispose();
                json = null;
            }

            Dictionary<string, object> ParseObject() {
                Dictionary<string, object> table = new Dictionary<string, object>();

                // ditch opening brace
                json.Read();

                // {
                while (true) {
                    switch (NextToken) {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.CURLY_CLOSE:
                        return table;
                    default:
                        // name
                        string name = ParseString();
                        if (name == null) {
                            return null;
                        }

                        // :
                        if (NextToken != TOKEN.COLON) {
                            return null;
                        }
                        // ditch the colon
                        json.Read();

                        // value
                        table[name] = ParseValue();
                        break;
                    }
                }
            }

            List<object> ParseArray() {
                List<object> array = new List<object>();

                // ditch opening bracket
                json.Read();

                // [
                var parsing = true;
                while (parsing) {
                    TOKEN nextToken = NextToken;

                    switch (nextToken) {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.SQUARED_CLOSE:
                        parsing = false;
                        break;
                    default:
                        object value = ParseByToken(nextToken);

                        array.Add(value);
                        break;
                    }
                }

                return array;
            }

            object ParseValue() {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            object ParseByToken(TOKEN token) {
                switch (token) {
                case TOKEN.STRING:
                    return ParseString();
                case TOKEN.NUMBER:
                    return ParseNumber();
                case TOKEN.CURLY_OPEN:
                    return ParseObject();
                case TOKEN.SQUARED_OPEN:
                    return ParseArray();
                case TOKEN.TRUE:
                    return true;
                case TOKEN.FALSE:
                    return false;
                case TOKEN.NULL:
                    return null;
                default:
                    return null;
                }
            }

            string ParseString() {
                StringBuilder s = new StringBuilder();
                char c;

                // ditch opening quote
                json.Read();

                bool parsing = true;
                while (parsing) {

                    if (json.Peek() == -1) {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c) {
                    case '"':
                        parsing = false;
                        break;
                    case '\\':
                        if (json.Peek() == -1) {
                            parsing = false;
                            break;
                        }

                        c = NextChar;
                        switch (c) {
                        case '"':
                        case '\\':
                        case '/':
                            s.Append(c);
                            break;
                        case 'b':
                            s.Append('\b');
                            break;
                        case 'f':
                            s.Append('\f');
                            break;
                        case 'n':
                            s.Append('\n');
                            break;
                        case 'r':
                            s.Append('\r');
                            break;
                        case 't':
                            s.Append('\t');
                            break;
                        case 'u':
                            var hex = new char[4];

                            for (int i=0; i< 4; i++) {
                                hex[i] = NextChar;
                            }

                            s.Append((char) Convert.ToInt32(new string(hex), 16));
                            break;
                        }
                        break;
                    default:
                        s.Append(c);
                        break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber() {
                string number = NextWord;

                if (number.IndexOf('.') == -1) {
                    long parsedInt;
                    Int64.TryParse(number, out parsedInt);
                    return parsedInt;
                }

                double parsedDouble;
                Double.TryParse(number, out parsedDouble);
                return parsedDouble;
            }

            void EatWhitespace() {
                while (Char.IsWhiteSpace(PeekChar)) {
                    json.Read();

                    if (json.Peek() == -1) {
                        break;
                    }
                }
            }

            char PeekChar {
                get {
                    return Convert.ToChar(json.Peek());
                }
            }

            char NextChar {
                get {
                    return Convert.ToChar(json.Read());
                }
            }

            string NextWord {
                get {
                    StringBuilder word = new StringBuilder();

                    while (!IsWordBreak(PeekChar)) {
                        word.Append(NextChar);

                        if (json.Peek() == -1) {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            TOKEN NextToken {
                get {
                    EatWhitespace();

                    if (json.Peek() == -1) {
                        return TOKEN.NONE;
                    }

                    switch (PeekChar) {
                    case '{':
                        return TOKEN.CURLY_OPEN;
                    case '}':
                        json.Read();
                        return TOKEN.CURLY_CLOSE;
                    case '[':
                        return TOKEN.SQUARED_OPEN;
                    case ']':
                        json.Read();
                        return TOKEN.SQUARED_CLOSE;
                    case ',':
                        json.Read();
                        return TOKEN.COMMA;
                    case '"':
                        return TOKEN.STRING;
                    case ':':
                        return TOKEN.COLON;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        return TOKEN.NUMBER;
                    }

                    switch (NextWord) {
                    case "false":
                        return TOKEN.FALSE;
                    case "true":
                        return TOKEN.TRUE;
                    case "null":
                        return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj) {
            return Serializer.Serialize(obj);
        }

        sealed class Serializer {
            StringBuilder builder;

            Serializer() {
                builder = new StringBuilder();
            }

            public static string Serialize(object obj) {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            void SerializeValue(object value) {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null) {
                    builder.Append("null");
                } else if ((asStr = value as string) != null) {
                    SerializeString(asStr);
                } else if (value is bool) {
                    builder.Append((bool) value ? "true" : "false");
                } else if ((asList = value as IList) != null) {
                    SerializeArray(asList);
                } else if ((asDict = value as IDictionary) != null) {
                    SerializeObject(asDict);
                } else if (value is char) {
                    SerializeString(new string((char) value, 1));
                } else {
                    SerializeOther(value);
                }
            }

            void SerializeObject(IDictionary obj) {
                bool first = true;

                builder.Append('{');

                foreach (object e in obj.Keys) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList anArray) {
                builder.Append('[');

                bool first = true;

                foreach (object obj in anArray) {
                    if (!first) {
                        builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str) {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray) {
                    switch (c) {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126)) {
                            builder.Append(c);
                        } else {
                            builder.Append("\\u");
                            builder.Append(codepoint.ToString("x4"));
                        }
                        break;
                    }
                }

                builder.Append('\"');
            }

            void SerializeOther(object value) {
                // NOTE: decimals lose precision during serialization.
                // They always have, I'm just letting you know.
                // Previously floats and doubles lost precision too.
                if (value is float) {
                    builder.Append(((float) value).ToString("R"));
                } else if (value is int
                    || value is uint
                    || value is long
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong) {
                    builder.Append(value);
                } else if (value is double
                    || value is decimal) {
                    builder.Append(Convert.ToDouble(value).ToString("R"));
                } else {
                    SerializeString(value.ToString());
                }
            }
        }
    }


}
