﻿using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class LazyParser<T> : Parser<T>
    {
        internal LazyParser(Func<Parser<T>> factory) : base("Lazy")
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser<T>> _factory;
        Parser<T> _cache;
        protected override ResultCore<T> DoParse(Reader src)
        {
            return (_cache ?? (_cache = _factory())).Parse(src);
        }
    }
}
