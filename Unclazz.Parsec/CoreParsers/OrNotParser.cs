using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OrNotParser<T> : Parser<T>
    {
        internal OrNotParser(IParser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly IParser<T> _original;

        public override ParseResult<T> Parse(Reader input)
        {
            input.Mark();
            var p = input.Position;
            var result = _original.Parse(input);
            if (result.Successful)
            {
                input.Unmark();
                return result;
            }
            input.Reset();
            input.Unmark();
            return Success(p);
        }
        public override string ToString()
        {
            return string.Format("OrNot({0})", _original);
        }
    }
}
