using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class OptionalParser<T> : Parser<Optional<T>>
    {
        internal OptionalParser(Parser<T> original) : base("OrNot")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore<Optional<T>> DoParse(Reader src)
        {
            src.Mark();
            var result = _original.Parse(src);
            if (result.Successful)
            {
                src.Unmark();
                return result.Map(a => new Optional<T>(a));
            }
            src.Reset(true);
            return Success(new Optional<T>());
        }
    }
}
