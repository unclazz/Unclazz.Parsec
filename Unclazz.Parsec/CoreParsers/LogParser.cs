using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LogParser<T> : Parser<T>
    {
        internal LogParser(Parser<T> original, Action<string> logger)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        readonly Parser<T> _original;
        readonly Action<string> _logger;

        public override ParseResult<T> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _original.Parse(input);
            _logger(leftResult.ToString());
            return leftResult;
        }
        public override string ToString()
        {
            return string.Format("Log({0})", _original);
        }
    }
}
