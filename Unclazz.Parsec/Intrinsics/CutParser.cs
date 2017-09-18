using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class CutParser<T> : Parser<T>
    {
        internal CutParser(Parser<T> original) : base("Cut")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore<T> DoParse(Context ctx)
        {
            var r = _original.Parse(ctx);
            if (r.Successful) return r.AllowBacktrack(false);
            return r.AllowBacktrack(true);
        }
    }
    sealed class CutParser : Parser
    {
        internal CutParser(Parser original) : base("Cut")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Context ctx)
        {
            var r = _original.Parse(ctx);
            if (r.Successful) return r.AllowBacktrack(false);
            return r.AllowBacktrack(true);
        }
    }
}
