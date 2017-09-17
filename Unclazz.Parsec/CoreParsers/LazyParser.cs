using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LazyParser : Parser
    {
        internal LazyParser(Func<Parser> factory) : base("Lazy")
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser> _factory;
        Parser _cache;
        protected override ResultCore DoParse(Context ctx)
        {
            return (_cache ?? (_cache = _factory())).Parse(ctx);
        }
    }
}
