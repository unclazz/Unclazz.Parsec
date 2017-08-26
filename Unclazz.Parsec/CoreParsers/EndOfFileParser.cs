namespace Unclazz.Parsec.CoreParsers
{
    sealed class EndOfFileParser : Parser
    {
        public override ParseResult<Nil> Parse(Reader input)
        {
            return input.EndOfFile
                ? Success(input.Position) : Failure(input.Position, 
                string.Format("expected EOF but found '{0}'", (char)input.Peek()));
        }
        public override string ToString()
        {
            return string.Format("EndOfFile()");
        }
    }
}
