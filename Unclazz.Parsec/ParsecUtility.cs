using System;
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
            "NUL", "SOH", "STX", "ETX", "EOT",
            "ENQ", "ACK", "BEL", "BS", "HT",
            "LF", "VT", "FF", "CR", "SO",
            "SI", "DLE", "DC1", "DC2", "DC3",
            "DC4", "NAK", "SYN", "ETB", "CAN",
            "EM", "SUB", "ESC", "FS", "GS",
            "RS", "US"
        };
        public static string CharString(int ch)
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
            return string.Format("{0} (codepoint = {1})", s, ch);
        }
    }
}
