using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LogParser<T> : Parser<T>
    {
        internal LogParser(IParser<T> target, Action<string> logger)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        readonly IParser<T> _target;
        readonly Action<string> _logger;

        public override ParseResult<T> Parse(ParserInput input)
        {
            try
            {
                WriteLine("--------------------------");
                WriteLine("Target Type         : {0} ", ParsecUtility.ObjectTypeToString(_target));
                WriteLine("Target Content      : {0} ", _target);
                WriteLine("Pre-Parse Position  : {0} ", input.Position);
                WriteLine("Pre-Parse Char      : {0} ", ParsecUtility.CharToString(input.Peek()));

                var r = _target.Parse(input);

                WriteLine("Parse Result        : {0}", r);

                WriteLine("Post-Parse Position : {0} ", input.Position);
                WriteLine("Post-Parse Char     : {0} ", ParsecUtility.CharToString(input.Peek()));

                return r;
            }
            catch
            {
                throw;
            }
        }

        void WriteLine(string format, params object[] args)
        {
            _logger(string.Format(format, args));
        }

        public override string ToString()
        {
            return string.Format("Log({0})", _target);
        }
    }
}
