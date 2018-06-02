using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<Reader, ResultCore<T>> func) : base("Delegate")
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(Func<Reader, Result<T>> func) : base("Delegate")
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ResultCore<T>> _delegate1;
        readonly Func<Reader, Result<T>> _delegate2;

        protected override ResultCore<T> DoParse(Reader src)
        {
            if (_delegate1 == null) return _delegate2(src);
            else return _delegate1(src);
        }
    }
    sealed class DelegateParser : Parser
    {
        internal DelegateParser(Func<Reader, ResultCore> func) : base("Delegate")
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(Func<Reader, Result> func) : base("Delegate")
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ResultCore> _delegate1;
        readonly Func<Reader, Result> _delegate2;

        protected override ResultCore DoParse(Reader src)
        {
            if (_delegate1 == null) return _delegate2(src);
            else return _delegate1(src);
        }
    }
}
