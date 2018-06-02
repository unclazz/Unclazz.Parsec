using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class NotParser<T> : Parser
    {
        internal NotParser(Parser<T> original) : base("Not")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore DoParse(Reader src)
        {
            src.Mark();
            var p = src.Position;
            var originalResult = _original.Parse(src);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                src.Reset();
                src.Unmark();
                return Success(originalResult.CanBacktrack);
            }
        }
    }
    sealed class NotParser : Parser
    {
        internal NotParser(Parser original) : base("Not")
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Reader src)
        {
            src.Mark();
            var originalResult = _original.Parse(src);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                src.Reset(true);
                return Success(originalResult.CanBacktrack);
            }
        }
    }
}
