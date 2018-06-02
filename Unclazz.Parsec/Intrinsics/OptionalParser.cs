using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class OptionalParser : Parser
    {
        internal OptionalParser(Parser original) : base("OrNot")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Reader src)
        {
            src.Mark();
            var result = _original.Parse(src);
            if (result.Successful)
            {
                src.Unmark();
                return result;
            }
            src.Reset(true);
            return Success();
        }
    }
}
