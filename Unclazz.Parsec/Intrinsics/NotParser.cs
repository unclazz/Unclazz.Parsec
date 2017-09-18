using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class NotParser<T> : Parser
    {
        internal NotParser(Parser<T> original) : base("Not")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var p = ctx.Source.Position;
            var originalResult = _original.Parse(ctx);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                ctx.Source.Reset();
                ctx.Source.Unmark();
                return Success(originalResult.CanBacktrack);
            }
        }
    }
    sealed class NotParser : Parser
    {
        internal NotParser(Parser original) : base("Not")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var originalResult = _original.Parse(ctx);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                ctx.Source.Reset(true);
                return Success(originalResult.CanBacktrack);
            }
        }
    }
}
