using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class TypedParser<T> : Parser<T>
    {
        internal TypedParser(Parser original) : this(original, default(T)) { }
        internal TypedParser(Parser original, T value) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _value = value;
        }
        readonly Parser _original;
        readonly T _value;
        protected override ResultCore<T> DoParse(Reader input)
        {
            return _original.Parse(input).Typed(_value);
        }
        public override string ToString()
        {
            return _original.ToString();
        }
    }
}
