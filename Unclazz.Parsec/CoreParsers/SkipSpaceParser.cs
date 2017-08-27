using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class SkipSpaceParser : Parser
    {
        static IParserConfiguration CopyAndModify(IParserConfiguration factory, bool skipOnOff)
        {
            var copy = new ParserFactory(factory);
            copy.SetNonSignificant(skipOnOff ? Parsers.WhileSpaceAndControls : null);
            return copy;
        }

        internal SkipSpaceParser(IParserConfiguration conf, Parser<Nil> original, bool onOff) : base(CopyAndModify(conf, onOff))
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _onOff = onOff;
            Configure(a => a.SetNonSignificant(onOff ? Parsers.WhileSpaceAndControls : null));
        }
        readonly Parser<Nil> _original;
        readonly bool _onOff;
        protected override ParseResult<Nil> DoParse(Reader input) => _original.Parse(input);
        public override string ToString()
        {
            return string.Format("SkipSpace({0}, onOff = {1})", _original, _onOff);
        }
    }
    sealed class SkipSpaceParser<T> : Parser<T>
    {
        static IParserConfiguration CopyAndModify(IParserConfiguration factory, bool skipOnOff)
        {
            var copy = new ParserFactory(factory);
            copy.SetNonSignificant(skipOnOff ? Parsers.WhileSpaceAndControls : null);
            return copy;
        }

        internal SkipSpaceParser(IParserConfiguration conf, Parser<T> original, bool onOff) : base(CopyAndModify(conf, onOff))
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _onOff = onOff;
            Configure(a => a.SetNonSignificant(onOff ? Parsers.WhileSpaceAndControls : null));
        }
        readonly Parser<T> _original;
        readonly bool _onOff;
        protected override ParseResult<T> DoParse(Reader input) => _original.Parse(input);
        public override string ToString()
        {
            return string.Format("SkipSpace({0}, onOff = {1})", _original, _onOff);
        }
    }
}
