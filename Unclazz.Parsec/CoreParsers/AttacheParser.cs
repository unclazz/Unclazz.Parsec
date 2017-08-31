using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class AttacheParser<U> : Parser<U>
    {
        internal AttacheParser(IParserConfiguration conf, Parser original, U defaultValue) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _default = defaultValue;
        }
        readonly Parser _original;
        readonly U _default;
        protected override ResultCore<U> DoParse(Reader input)
        {
            return _original.Parse(input).AttachValue(_default);
        }
        public override string ToString()
        {
            return string.Format("Attach<{1}>({0})", _original,
                ParsecUtility.TypeToString(typeof(U)));
        }
    }
}
