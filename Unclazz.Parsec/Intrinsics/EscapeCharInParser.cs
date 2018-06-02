using System.Collections.Generic;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class EscapeCharInParser : Parser<char>
    {
        internal EscapeCharInParser(IEnumerable<char> charIn, char prefix = '\\') : base("CharEscape")
        {
            _charIn = Char(prefix) & CharIn(charIn).Capture();
        }
        readonly Parser<char> _charIn;
        protected override ResultCore<char> DoParse(Reader src)
        {
            return _charIn.Parse(src);
        }
    }
}
