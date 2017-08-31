using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(IParserConfiguration conf, Func<Reader, ResultCore<T>> func) : base(conf)
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(IParserConfiguration conf, Func<Reader, Result<T>> func) : base(conf)
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ResultCore<T>> _delegate1;
        readonly Func<Reader, Result<T>> _delegate2;

        protected override ResultCore<T> DoParse(Reader input)
        {
            if (_delegate1 == null) return _delegate2(input);
            else return _delegate1(input);
        }
        public override string ToString()
        {
            return string.Format("For<{0}>()", (_delegate1 ?? _delegate1).GetType());
        }
    }
    sealed class DelegateParser : Parser
    {
        internal DelegateParser(IParserConfiguration conf, Func<Reader, ResultCore> func) : base(conf)
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(IParserConfiguration conf, Func<Reader, Result> func) : base(conf)
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ResultCore> _delegate1;
        readonly Func<Reader, Result> _delegate2;

        protected override ResultCore DoParse(Reader input)
        {
            if (_delegate1 == null) return _delegate2(input);
            else return _delegate1(input);
        }
        public override string ToString()
        {
            return string.Format("For<{0}>()", (_delegate1 ?? _delegate1).GetType());
        }
    }
}
