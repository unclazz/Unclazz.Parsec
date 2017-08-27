using System;

namespace Unclazz.Parsec
{
    public interface IParserConfiguration
    {
        Action<string> Logger { get; }
        Parser NonSignificant { get; }

        IParserConfiguration Copy();
    }
}