﻿using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class LazyParser : Parser
    {
        internal LazyParser(Func<Parser> factory) : base("Lazy")
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser> _factory;
        Parser _cache;
        protected override ResultCore DoParse(Reader src)
        {
            return (_cache ?? (_cache = _factory())).Parse(src);
        }
    }
}
