using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class SkipParser : Parser
    {
        internal SkipParser(Parser original, CharClass target) : base("Skip")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _clazz = target;
        }
        readonly Parser _original;
        readonly CharClass _clazz;
        protected override ResultCore DoParse(Context ctx) => _original
            .Parse(ctx.Configure(a => a.SetSkipTarget(_clazz)));
    }
    sealed class SkipParser<T> : Parser<T>
    {
        internal SkipParser(Parser<T> original, CharClass target) : base("Skip")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _clazz = target;
        }
        readonly Parser<T> _original;
        readonly CharClass _clazz;
        protected override ResultCore<T> DoParse(Context ctx) => _original
            .Parse(ctx.Configure(a => a.SetSkipTarget(_clazz)));
    }
}
