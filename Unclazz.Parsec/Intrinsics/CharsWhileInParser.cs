using System;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class CharsWhileInParser : Parser
    {
        internal CharsWhileInParser(CharClass clazz, int min) : base("CharsWhileIn")
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            _min = min;
        }

        readonly int _min;
        readonly CharClass _clazz;

        protected override ResultCore DoParse(Context ctx)
        {
            var count = 0;
            while (!ctx.Source.EndOfFile)
            {
                var ch = (char)ctx.Source.Peek();
                if (!_clazz.Contains(ch)) break;
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
