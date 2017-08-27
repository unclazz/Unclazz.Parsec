using Unclazz.Parsec;

namespace Example.Unclazz.Parsec.Math
{
    sealed class NumberParser : Parser<double>
    {
        readonly Parser sign;
        readonly Parser digits;
        readonly Parser integral;
        readonly Parser fractional;
        readonly Parser exponent;
        readonly Parser<double> number;

        public NumberParser()
        {
            sign = CharIn("+-").OrNot();
            digits = CharsWhileIn("0123456789", min: 0);
            integral = Char('0') | (CharBetween('1', '9') & digits);
            fractional = Char('.') & digits;
            exponent = CharIn("eE") & (sign) & (digits);
            number = ((sign.OrNot() & integral & fractional.OrNot() & exponent.OrNot()).Capture()).Map(double.Parse);
        }

        protected override ParseResult<double> DoParse(Reader input)
        {
            return number.Parse(input);
        }
    }
}
