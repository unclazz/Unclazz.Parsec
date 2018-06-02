using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class CharsWhileBetweenParser : Parser
    {
        internal CharsWhileBetweenParser(char start, char end, int min) : base("CharsWhileBetween")
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            _min = min;
            _start = start <= end ? start : end;
            _end = start <= end ? end : start;
        }

        readonly int _min;
        readonly char _start;
        readonly char _end;

        protected override ResultCore DoParse(Reader src)
        {
            var count = 0;
            while (!src.EndOfFile)
            {
                var ch = src.Peek();
                if (ch < _start || _end < ch) break;
                src.Read();
                count++;
            }
            if (_min <= count)
            {
                return Success();
            }
            else
            {
                var m = string.Format("expected that length of char sequence is" +
                    " greater than or equal {0}, but actualy it is {1}.",
                    _min, count);
                return Failure(m);
            }
        }
    }
}
