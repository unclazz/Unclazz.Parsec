namespace Unclazz.Parsec.CoreParsers
{
    sealed class EndOfFileParser : Parser
    {
        internal EndOfFileParser(IParserConfiguration conf) : base(conf) { }
        protected override ResultCore DoParse(Reader input)
        {
            return input.EndOfFile ? Success() 
                : Failure(string.Format("expected EOF but found '{0}'", (char)input.Peek()));
        }
        public override string ToString()
        {
            return string.Format("EndOfFile()");
        }
    }
}
