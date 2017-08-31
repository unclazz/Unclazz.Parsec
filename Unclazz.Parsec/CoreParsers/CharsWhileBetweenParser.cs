using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharsWhileBetweenParser : Parser
    {
        internal CharsWhileBetweenParser(IParserConfiguration conf, char start, char end, int min) : base(conf)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            _min = min;
            _start = start <= end ? start : end;
            _end = start <= end ? end : start;
        }

        readonly int _min;
        readonly char _start;
        readonly char _end;

        protected override ResultCore DoParse(Reader input)
        {
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
        public override string ToString()
        {
            return string.Format("CharsWhileBetween(start = {0}, end = {1}, min = {2})", _start, _end, _min);
        }
    }
}
