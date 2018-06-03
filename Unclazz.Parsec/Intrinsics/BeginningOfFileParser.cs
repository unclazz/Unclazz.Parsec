﻿namespace Unclazz.Parsec.Intrinsics
{
    /// <summary>
    /// BOFにのみパッチするパーサーです。
    /// このパーサーはパースにあたって文字位置の変更を行いません。
    /// </summary>
    sealed class BeginningOfFileParser : Parser
    {
        internal BeginningOfFileParser() : base("BOF") { }
        protected override ResultCore DoParse(Reader src)
        {
            var p = src.Position;
            if (p.Index == 0) return Success();
            else return Failure(string.Format("BOF(index = 0) expected but already index is {0}.", p.Index));
        }
    }
}
