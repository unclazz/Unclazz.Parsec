namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// BOFにのみパッチするパーサーです。
    /// このパーサーはパースにあたって文字位置の変更を行いません。
    /// </summary>
    sealed class BeginningOfFileParser : Parser
    {
        internal BeginningOfFileParser(IParserConfiguration conf) : base("BOF") { }
        protected override ResultCore DoParse(Context ctx)
        {
            var p = ctx.Source.Position;
            if (p.Index == 0) return Success();
            else return Failure(string.Format("BOF(index = 0) expected but already index is {0}.", p.Index));
        }
    }
}
