using Unclazz.Parsec;

namespace Example.Unclazz.Parsec.Math
{
    sealed class NumberParser : Parser<double>
    {
        readonly Parser<double> _number;
        public NumberParser()
        {
            Parser sign = CharIn("+-").OrNot();
            Parser digits = CharsWhileIn("0123456789", min: 0);
            Parser integral = Char('0') | (CharBetween('1', '9') & digits);
            Parser fractional = Char('.') & digits;
            Parser exponent = CharIn("eE") & sign & digits;
            // 始まりの符号はあるかもしれない（ないかもしれない）
            // 整数は必須
            // 小数点以下はあるかもしれない（ないかもしれない）
            // 指数はあるかもしれない（ないかもしれない）
            // 以上の文字列をキャプチャし、浮動小数点数に変換
            Parser<string> raw = (sign & integral & fractional.OrNot() & exponent.OrNot()).Capture();
            _number = raw.Map(double.Parse);
        }
        protected override ParseResult<double> DoParse(Reader input)
        {
            return _number.Parse(input);
        }
    }
}
