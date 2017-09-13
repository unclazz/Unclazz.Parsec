using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class HexDigitsParser : Parser<int>
    {
        internal HexDigitsParser(int min = 1, int max = -1, int exactly = -1)
        {
            if (min < 1) throw new ArgumentOutOfRangeException(nameof(min));
            _hexDigits = CharIn(CharClass.HexDigit).Capture()
                .Map(HexDigits_HexDigitToInt).Repeat(min, max, exactly)
                .Aggregate((a, b) => a * 16 + b);
        }
        protected override ResultCore<int> DoParse(Reader input)
        {
            return _hexDigits.Parse(input);
        }
        readonly Parser<int> _hexDigits;
        int HexDigits_HexDigitToInt(char ch)
        {
            if ('0' <= ch && ch <= '9') return ch - '0';
            else if ('A' <= ch && ch <= 'F') return ch - 'A' + 10;
            else if ('a' <= ch && ch <= 'a') return ch - 'a' + 10;
            else throw new ArgumentException("invalid character as hex-digit.");
        }
    }
}
