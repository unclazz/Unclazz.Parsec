using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OptionalParser : Parser
    {
        internal OptionalParser(Parser original) : base("OrNot")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var result = _original.Parse(ctx);
            if (result.Successful)
            {
                ctx.Source.Unmark();
                return result;
            }
            ctx.Source.Reset(true);
            return Success();
        }
    }
}
