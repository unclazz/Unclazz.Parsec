namespace Unclazz.Parsec.CoreParsers
{
    sealed class BeginningOfFileParser : Parser<string>
    {
        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            return p.Index == 0  ? Success(p)
                : Failure(p, string.Format("expected BOF but already index is {0}", p.Index));
        }
        public override string ToString()
        {
            return string.Format("BeginningOfFile()");
        }
    }
}
