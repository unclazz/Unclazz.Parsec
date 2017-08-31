using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CastParser<T> : Parser
    {
        internal CastParser(IParserConfiguration conf, Parser<T> original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser<T> _original;
        protected override ResultCore DoParse(Reader input)
        {
            return _original.Parse(input).DetachValue();
        }
        public override string ToString()
        {
            return string.Format("Cast({0})", _original);
        }
    }
}
