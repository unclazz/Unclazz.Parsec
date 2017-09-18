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

        protected override ResultCore<Optional<T>> DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var result = _original.Parse(ctx);
            if (result.Successful)
            {
                ctx.Source.Unmark();
                return result.Map(a => new Optional<T>(a));
            }
            ctx.Source.Reset(true);
            return Success(new Optional<T>());
        }
    }
}
