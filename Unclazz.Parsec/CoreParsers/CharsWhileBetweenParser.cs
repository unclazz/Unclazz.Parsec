using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.CoreParsers
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

        protected override ResultCore DoParse(Context ctx)
        {
            var count = 0;
            while (!ctx.Source.EndOfFile)
            {
                var ch = ctx.Source.Peek();
                if (ch < _start || _end < ch) break;
                ctx.Source.Read();
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
