using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class LookaheadParser : Parser
    {
        internal LookaheadParser(Parser original) : base("Lookahead")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser _original;
        protected override ResultCore DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var res = _original.Parse(ctx);
            ctx.Source.Reset(true);
            return res;
        }
    }
}
