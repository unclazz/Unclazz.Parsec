using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class EscapeCharInParser : Parser<char>
    {
        internal EscapeCharInParser(IEnumerable<char> charIn, char prefix = '\\') : base("CharEscape")
        {
            _charIn = Char(prefix) & CharIn(charIn).Capture();
        }
        readonly Parser<char> _charIn;
        protected override ResultCore<char> DoParse(Context ctx)
        {
            return _charIn.Parse(ctx);
        }
    }
}
