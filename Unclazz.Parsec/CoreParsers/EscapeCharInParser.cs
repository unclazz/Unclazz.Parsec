using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class EscapeCharInParser : Parser<char>
    {
        internal EscapeCharInParser(IEnumerable<char> charIn, char prefix = '\\')
        {
            _charIn = Char(prefix) & CharIn(charIn).Capture();
        }
        readonly Parser<char> _charIn;
        protected override ResultCore<char> DoParse(Reader input)
        {
            return _charIn.Parse(input);
        }
    }
}
