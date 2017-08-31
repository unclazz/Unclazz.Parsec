using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class UntypedParser<TSource> : Parser
    {
        internal UntypedParser(Parser<TSource> original) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser<TSource> _original;
        protected override ResultCore DoParse(Reader input)
        {
            return _original.Parse(input).DetachValue();
        }
        public override string ToString()
        {
            return _original.ToString();
        }
    }
}
