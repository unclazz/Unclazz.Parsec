using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class NotParser<T> : NilParser
    {
        internal NotParser(Parser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        public override ParseResult<Nil> Parse(Reader input)
        {
            input.Mark();
            var p = input.Position;
            var originalResult = _original.Parse(input);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(p, m, originalResult.CanBacktrack);
            }
            else
            {
                input.Reset();
                input.Unmark();
                return Success(p, canBacktrack: originalResult.CanBacktrack);
            }
        }

        public override string ToString()
        {
            return string.Format("Not({0})", _original);
        }
    }
}
