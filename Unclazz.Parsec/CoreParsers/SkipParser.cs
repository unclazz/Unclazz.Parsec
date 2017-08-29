using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class SkipParser : Parser
    {
        static IParserConfiguration CopyAndModify(IParserConfiguration factory, bool skipOnOff, CharClass target)
        {
            var copy = new ParserFactory(factory);
            copy.SetAutoSkip(skipOnOff);
            copy.SetSkipTarget(target ?? CharClass.SpaceAndControl);
            return copy;
        }

        internal SkipParser(IParserConfiguration conf, Parser<Nil> original, 
            bool onOff, CharClass target) : base(CopyAndModify(conf, onOff, target))
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _onOff = onOff;
        }
        readonly Parser<Nil> _original;
        readonly bool _onOff;
        readonly CharClass _target;
        protected override ParseResult<Nil> DoParse(Reader input) => _original.Parse(input);
        public override string ToString()
        {
            return string.Format("Skip({0}, onOff = {1})", _original, _onOff);
        }
    }
    sealed class SkipSpaceParser<T> : Parser<T>
    {
        static IParserConfiguration CopyAndModify(IParserConfiguration factory, bool skipOnOff, CharClass target)
        {
            var copy = new ParserFactory(factory);
            copy.SetAutoSkip(skipOnOff);
            copy.SetSkipTarget(target ?? CharClass.SpaceAndControl);
            return copy;
        }

        internal SkipSpaceParser(IParserConfiguration conf, Parser<T> original,
            bool onOff, CharClass target) : base(CopyAndModify(conf, onOff, target))
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _onOff = onOff;
        }
        readonly Parser<T> _original;
        readonly bool _onOff;
        protected override ParseResult<T> DoParse(Reader input) => _original.Parse(input);
        public override string ToString()
        {
            return string.Format("Skip({0}, onOff = {1})", _original, _onOff);
        }
    }
}
