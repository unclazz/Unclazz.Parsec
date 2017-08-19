using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharsWhileInParser : Parser
    {
        internal CharsWhileInParser(CharClass clazz, int min)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            _min = min;
        }

        readonly int _min;
        readonly CharClass _clazz;

        public override ParseResult<X> Parse(ParserInput input)
        {
            var p = input.Position;
            var count = 0;
            while (!input.EndOfFile)
            {
                var ch = (char)input.Peek();
                if (!_clazz.Contains(ch)) break;
                input.Read();
                count++;
            }
            if (_min <= count)
            {
                return Success(p);
            }
            else
            {
                var m = string.Format("expected that length of char sequence is" +
                    " greater than or equal {0}, but actualy it is {1}.",
                    _min, count);
                return Failure(p, m);
            }
        }
        public override string ToString()
        {
            return string.Format("CharsWhileIn({0}, min = {1})", _clazz, _min);
        }
    }
}
