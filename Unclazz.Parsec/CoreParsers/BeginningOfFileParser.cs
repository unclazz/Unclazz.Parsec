namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// BOFにのみパッチするパーサーです。
    /// このパーサーはパースにあたって文字位置の変更を行いません。
    /// </summary>
    sealed class BeginningOfFileParser : Parser
    {
        internal BeginningOfFileParser(IParserConfiguration conf) : base(conf) { }
        protected override ResultCore DoParse(Reader input)
        {
            var p = input.Position;
            if (p.Index == 0) return Success();
            else return Failure(string.Format("expected BOF but already index is {0}", p.Index));
        }
        public override string ToString()
        {
            return string.Format("BeginningOfFile()");
        }
    }
}
