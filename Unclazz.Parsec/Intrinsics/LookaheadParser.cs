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
        protected override ResultCore DoParse(Reader src)
        {
            src.Mark();
            var res = _original.Parse(src);
            src.Reset(true);
            return res;
        }
    }
}
