using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LookaheadParser : Parser
    {
        internal LookaheadParser(Parser original) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser _original;
        protected override ResultCore DoParse(Reader input)
        {
            var b = true;
            var b2 = !!b;
            input.Mark();
            var res = _original.Parse(input);
            input.Reset();
            return res;
        }
        public override string ToString()
        {
            return string.Format("Lookahead({0})", _original);
        }
    }
}
