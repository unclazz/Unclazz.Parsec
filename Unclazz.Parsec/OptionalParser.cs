using System;

namespace Unclazz.Parsec
{
    sealed class OptionalParser<T> : Parser<T>
    {
        internal OptionalParser(Parser<T> original) : this(original, default(T)) { }
        internal OptionalParser(Parser<T> original, T defaultValue)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _default = defaultValue;
        }

        readonly T _default;
        readonly Parser<T> _original;

        public override ParseResult<T> Parse(ParserInput input)
        {
            input.Mark();
            var p = input.Position;
            var result = _original.Parse(input);
            if (result.Successful)
            {
                input.Unmark();
                return ParseResult.OfSuccess(p, result.Value);
            }
            input.Reset();
            input.Unmark();
            return ParseResult.OfSuccess(p, _default);
        }
    }
}
