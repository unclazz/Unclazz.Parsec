using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharsWhileBetweenParser : NilParser
    {
        internal CharsWhileBetweenParser(char start, char end, int min)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            _min = min;
            _start = start <= end ? start : end;
            _end = start <= end ? end : start;
        }

        readonly int _min;
        readonly char _start;
        readonly char _end;

        public override ParseResult<Nil> Parse(Reader input)
        {
            var p = input.Position;
            var count = 0;
            while (!input.EndOfFile)
            {
                var ch = input.Peek();
                if (ch < _start || _end < ch) break;
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
            return string.Format("CharsWhileBetween(start = {0}, end = {1}, min = {2})", _start, _end, _min);
        }
    }
}
