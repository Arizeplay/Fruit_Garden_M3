﻿// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.JSONUtility
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yurowm.GameCore
{
  public static class JSONUtility
  {
    public static object Deserialize(string json) => json == null ? (object) null : JSONUtility.Parser.Parse(json);

    public static string Serialize(object obj) => JSONUtility.Serializer.Serialize(obj);

    private sealed class Parser : IDisposable
    {
      private const string WHITE_SPACE = " \t\n\r";
      private const string WORD_BREAK = " \t\n\r{}[],:\"";
      private StringReader json;

      private Parser(string jsonString) => this.json = new StringReader(jsonString);

      public static object Parse(string jsonString)
      {
        using (JSONUtility.Parser parser = new JSONUtility.Parser(jsonString))
          return parser.ParseValue();
      }

      public void Dispose()
      {
        this.json.Dispose();
        this.json = (StringReader) null;
      }

      private Dictionary<string, object> ParseObject()
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        this.json.Read();
        while (true)
        {
          JSONUtility.Parser.TOKEN nextToken;
          do
          {
            nextToken = this.NextToken;
            if (nextToken != JSONUtility.Parser.TOKEN.NONE)
            {
              if (nextToken == JSONUtility.Parser.TOKEN.CURLY_CLOSE)
                goto label_5;
            }
            else
              goto label_4;
          }
          while (nextToken == JSONUtility.Parser.TOKEN.COMMA);
          string key = this.ParseString();
          if (key != null)
          {
            if (this.NextToken == JSONUtility.Parser.TOKEN.COLON)
            {
              this.json.Read();
              dictionary[key] = this.ParseValue();
            }
            else
              goto label_9;
          }
          else
            goto label_7;
        }
label_4:
        return (Dictionary<string, object>) null;
label_5:
        return dictionary;
label_7:
        return (Dictionary<string, object>) null;
label_9:
        return (Dictionary<string, object>) null;
      }

      private List<object> ParseArray()
      {
        List<object> array = new List<object>();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          JSONUtility.Parser.TOKEN nextToken = this.NextToken;
          switch (nextToken)
          {
            case JSONUtility.Parser.TOKEN.NONE:
              return (List<object>) null;
            case JSONUtility.Parser.TOKEN.SQUARED_CLOSE:
              flag = false;
              continue;
            case JSONUtility.Parser.TOKEN.COMMA:
              continue;
            default:
              object byToken = this.ParseByToken(nextToken);
              array.Add(byToken);
              continue;
          }
        }
        return array;
      }

      private object ParseValue() => this.ParseByToken(this.NextToken);

      private object ParseByToken(JSONUtility.Parser.TOKEN token)
      {
        switch (token)
        {
          case JSONUtility.Parser.TOKEN.CURLY_OPEN:
            return (object) this.ParseObject();
          case JSONUtility.Parser.TOKEN.SQUARED_OPEN:
            return (object) this.ParseArray();
          case JSONUtility.Parser.TOKEN.STRING:
            return (object) this.ParseString();
          case JSONUtility.Parser.TOKEN.NUMBER:
            return this.ParseNumber();
          case JSONUtility.Parser.TOKEN.TRUE:
            return (object) true;
          case JSONUtility.Parser.TOKEN.FALSE:
            return (object) false;
          case JSONUtility.Parser.TOKEN.NULL:
            return (object) null;
          default:
            return (object) null;
        }
      }

      private string ParseString()
      {
        StringBuilder stringBuilder1 = new StringBuilder();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          if (this.json.Peek() == -1)
            break;
          char nextChar1 = this.NextChar;
          switch (nextChar1)
          {
            case '"':
              flag = false;
              continue;
            case '\\':
              if (this.json.Peek() == -1)
              {
                flag = false;
                continue;
              }
              char nextChar2 = this.NextChar;
              switch (nextChar2)
              {
                case '"':
                case '/':
                case '\\':
                  stringBuilder1.Append(nextChar2);
                  continue;
                case 'b':
                  stringBuilder1.Append('\b');
                  continue;
                case 'f':
                  stringBuilder1.Append('\f');
                  continue;
                case 'n':
                  stringBuilder1.Append('\n');
                  continue;
                case 'r':
                  stringBuilder1.Append('\r');
                  continue;
                case 't':
                  stringBuilder1.Append('\t');
                  continue;
                case 'u':
                  StringBuilder stringBuilder2 = new StringBuilder();
                  for (int index = 0; index < 4; ++index)
                    stringBuilder2.Append(this.NextChar);
                  stringBuilder1.Append((char) Convert.ToInt32(stringBuilder2.ToString(), 16));
                  continue;
                default:
                  continue;
              }
            default:
              stringBuilder1.Append(nextChar1);
              continue;
          }
        }
        return stringBuilder1.ToString();
      }

      private object ParseNumber()
      {
        string nextWord = this.NextWord;
        if (nextWord.IndexOf('.') == -1)
        {
          long result;
          long.TryParse(nextWord, out result);
          return (object) result;
        }
        double result1;
        double.TryParse(nextWord, out result1);
        return (object) result1;
      }

      private void EatWhitespace()
      {
        while (" \t\n\r".IndexOf(this.PeekChar) != -1)
        {
          this.json.Read();
          if (this.json.Peek() == -1)
            break;
        }
      }

      private char PeekChar => Convert.ToChar(this.json.Peek());

      private char NextChar => Convert.ToChar(this.json.Read());

      private string NextWord
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (" \t\n\r{}[],:\"".IndexOf(this.PeekChar) == -1)
          {
            stringBuilder.Append(this.NextChar);
            if (this.json.Peek() == -1)
              break;
          }
          return stringBuilder.ToString();
        }
      }

      private JSONUtility.Parser.TOKEN NextToken
      {
        get
        {
          this.EatWhitespace();
          if (this.json.Peek() == -1)
            return JSONUtility.Parser.TOKEN.NONE;
          switch (this.PeekChar)
          {
            case '"':
              return JSONUtility.Parser.TOKEN.STRING;
            case ',':
              this.json.Read();
              return JSONUtility.Parser.TOKEN.COMMA;
            case '-':
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
              return JSONUtility.Parser.TOKEN.NUMBER;
            case ':':
              return JSONUtility.Parser.TOKEN.COLON;
            case '[':
              return JSONUtility.Parser.TOKEN.SQUARED_OPEN;
            case ']':
              this.json.Read();
              return JSONUtility.Parser.TOKEN.SQUARED_CLOSE;
            case '{':
              return JSONUtility.Parser.TOKEN.CURLY_OPEN;
            case '}':
              this.json.Read();
              return JSONUtility.Parser.TOKEN.CURLY_CLOSE;
            default:
              string nextWord = this.NextWord;
              if (nextWord == "false")
                return JSONUtility.Parser.TOKEN.FALSE;
              if (nextWord == "true")
                return JSONUtility.Parser.TOKEN.TRUE;
              return nextWord == "null" ? JSONUtility.Parser.TOKEN.NULL : JSONUtility.Parser.TOKEN.NONE;
          }
        }
      }

      private enum TOKEN
      {
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
        NULL,
      }
    }

    private sealed class Serializer
    {
      private StringBuilder builder;

      private Serializer() => this.builder = new StringBuilder();

      public static string Serialize(object obj)
      {
        JSONUtility.Serializer serializer = new JSONUtility.Serializer();
        serializer.SerializeValue(obj);
        return serializer.builder.ToString();
      }

      private void SerializeValue(object value)
      {
        switch (value)
        {
          case null:
            this.builder.Append("null");
            break;
          case string str:
            this.SerializeString(str);
            break;
          case bool _:
            this.builder.Append(value.ToString().ToLower());
            break;
          case IList anArray:
            this.SerializeArray(anArray);
            break;
          case IDictionary dictionary:
            this.SerializeObject(dictionary);
            break;
          case char _:
            this.SerializeString(value.ToString());
            break;
          default:
            this.SerializeOther(value);
            break;
        }
      }

      private void SerializeObject(IDictionary obj)
      {
        bool flag = true;
        this.builder.Append('{');
        foreach (object key in (IEnumerable) obj.Keys)
        {
          if (!flag)
            this.builder.Append(',');
          this.SerializeString(key.ToString());
          this.builder.Append(':');
          this.SerializeValue(obj[key]);
          flag = false;
        }
        this.builder.Append('}');
      }

      private void SerializeArray(IList anArray)
      {
        this.builder.Append('[');
        bool flag = true;
        foreach (object an in (IEnumerable) anArray)
        {
          if (!flag)
            this.builder.Append(',');
          this.SerializeValue(an);
          flag = false;
        }
        this.builder.Append(']');
      }

      private void SerializeString(string str)
      {
        this.builder.Append('"');
        foreach (char ch in str.ToCharArray())
        {
          switch (ch)
          {
            case '\b':
              this.builder.Append("\\b");
              break;
            case '\t':
              this.builder.Append("\\t");
              break;
            case '\n':
              this.builder.Append("\\n");
              break;
            case '\f':
              this.builder.Append("\\f");
              break;
            case '\r':
              this.builder.Append("\\r");
              break;
            case '"':
              this.builder.Append("\\\"");
              break;
            case '\\':
              this.builder.Append("\\\\");
              break;
            default:
              int int32 = Convert.ToInt32(ch);
              if (int32 >= 32 && int32 <= 126)
              {
                this.builder.Append(ch);
                break;
              }
              this.builder.Append("\\u" + Convert.ToString(int32, 16).PadLeft(4, '0'));
              break;
          }
        }
        this.builder.Append('"');
      }

      private void SerializeOther(object value)
      {
        switch (value)
        {
          case float _:
          case int _:
          case uint _:
          case long _:
          case double _:
          case sbyte _:
          case byte _:
          case short _:
          case ushort _:
          case ulong _:
          case Decimal _:
            this.builder.Append(value.ToString());
            break;
          default:
            this.SerializeString(value.ToString());
            break;
        }
      }
    }
  }
}