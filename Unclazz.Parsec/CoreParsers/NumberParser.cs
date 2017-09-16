using System;
using Unclazz.Parsec;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class NumberParser : Parser<double>
    {
        readonly Parser<double> _number;
        public NumberParser()
        {
            var signOpt = CharIn("+-").OrNot();
            var digits = CharsWhileIn("0123456789", min: 0);
            var integral = Char('0') | (CharBetween('1', '9') & digits);
            var fractionalOpt = (Char('.') & digits).OrNot();
            var exponentOpt = (CharIn("eE") & signOpt & digits).OrNot();
            var raw = (signOpt & integral & fractionalOpt & exponentOpt).Capture();
            _number = raw.Map(double.Parse);
        }
        protected override ResultCore<double> DoParse(Reader input)
        {
            return _number.Parse(input);
        }
    }
}
