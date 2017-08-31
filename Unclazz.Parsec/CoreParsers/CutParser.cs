using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CutParser<T> : Parser<T>
    {
        internal CutParser(IParserConfiguration conf, Parser<T> original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore<T> DoParse(Reader input)
        {
            var r = _original.Parse(input);
            if (r.Successful) return r.AllowBacktrack(false);
            return r.AllowBacktrack(true);
        }

        public override string ToString()
        {
            return string.Format("Cut({0})", _original);
        }
    }
    sealed class CutParser : Parser
    {
        internal CutParser(IParserConfiguration conf, Parser original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Reader input)
        {
            var r = _original.Parse(input);
            if (r.Successful) return r.AllowBacktrack(false);
            return r.AllowBacktrack(true);
        }

        public override string ToString()
        {
            return string.Format("Cut({0})", _original);
        }
    }
}
