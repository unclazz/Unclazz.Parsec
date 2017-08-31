using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DummyParser<T> : Parser<T>
    {
        internal DummyParser(Parser original) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser _original;
        protected override ResultCore<T> DoParse(Reader input)
        {
            var pos = input.Position;
            var res = _original.Parse(input);
            if (res.Successful)
            {
                return Success(default(T), res.CanBacktrack);
            }
            else
            {
                return Failure(res.Message, res.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return _original.ToString();
        }
    }
}
