using System;

namespace Unclazz.Parsec
{
    sealed class CutParser<T> : Parser<T>
    {
        internal CutParser(Parser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        public override ParseResult<T> Parse(ParserInput input)
        {
            var originalResult = _original.Parse(input);
            if (originalResult.Successful) return originalResult.AllowBacktrack(false);
            return originalResult.AllowBacktrack(false);
        }

        public override string ToString()
        {
            return string.Format("Cut({0})", _original);
        }
    }
}
