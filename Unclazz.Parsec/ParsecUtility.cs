using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    static class ParsecUtility
    {
        static readonly string[] code0to31 = new[]
        {
            "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "HT",
            "LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3",
            "DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS",
            "RS", "US"
        };
        public static string CharToString(int ch)
        {
            if (ch == -1) return "EOF";
            string s = null;
            if (0 <= ch && ch <= 31)
            {
                s = code0to31[ch];
            }
            else if (ch == 127)
            {
                s = "DEL";
            }
            else
            {
                s = string.Format("'{0}'", (char)ch);
            }
            return string.Format("{0}({1})", s, ch);
        }
        public static string ValueToString<T>(T _value)
        {
            var str = _value as string;
            if (str != null) return str;
            var col = _value as IEnumerable;
            if (col == null)
            {
                return _value.ToString();
            }
            else
            {
                var buff = new StringBuilder();
                foreach (var e in col.Cast<object>().Select((o, i) => new { Index = i, Item = o }))
                {
                    if (e.Index > 0) buff.Append(',').Append(' ');
                    buff.Append(ValueToString(e.Item));
                }
                return buff.ToString();
            }
        }
        public static string ObjectTypeToString<T>(T value)
        {
            return TypeToString(value.GetType());
        }
        public static string TypeToString(Type type)
        {
            var buff = new StringBuilder();
            TypeToString(type, buff);
            return buff.ToString();
        }
        static void TypeToString(Type t, StringBuilder b)
        {
            var type = t;
            var typeName = type.Name;
            var typeArgs = type.GenericTypeArguments;
            b.Append(typeName);
            if (typeArgs.Length > 0)
            {
                b.Append('<');
                var initLength = b.Length;
                foreach (var a in typeArgs)
                {
                    if (b.Length > initLength) b.Append(',').Append(' ');
                    TypeToString(a, b);
                }
                b.Append('>');
            }
        }
        public static ResultCore Or(Reader input, Parser left, Parser right)
        {
            input.Mark();
            var leftResult = left.Parse(input);
            if (leftResult.Successful || !leftResult.CanBacktrack)
            {
                input.Unmark();
                return leftResult.AllowBacktrack(true);
            }
            input.Reset();
            input.Unmark();
            var rightResult = right.Parse(input);
            return rightResult.AllowBacktrack(true);
        }
    }
}
