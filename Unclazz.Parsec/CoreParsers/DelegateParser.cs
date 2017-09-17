using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<Context, ResultCore<T>> func) : base("Delegate")
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(Func<Context, Result<T>> func) : base("Delegate")
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Context, ResultCore<T>> _delegate1;
        readonly Func<Context, Result<T>> _delegate2;

        protected override ResultCore<T> DoParse(Context ctx)
        {
            if (_delegate1 == null) return _delegate2(ctx);
            else return _delegate1(ctx);
        }
    }
    sealed class DelegateParser : Parser
    {
        internal DelegateParser(Func<Context, ResultCore> func) : base("Delegate")
        {
            _delegate1 = func ?? throw new ArgumentNullException(nameof(func));
        }
        internal DelegateParser(Func<Context, Result> func) : base("Delegate")
        {
            _delegate2 = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Context, ResultCore> _delegate1;
        readonly Func<Context, Result> _delegate2;

        protected override ResultCore DoParse(Context ctx)
        {
            if (_delegate1 == null) return _delegate2(ctx);
            else return _delegate1(ctx);
        }
    }
}
