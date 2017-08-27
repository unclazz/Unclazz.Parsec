using System;

namespace Unclazz.Parsec
{
    public interface IParserConfigurer
    {
        IParserConfiguration SetLogger(Action<string> l);
        IParserConfiguration SetNonSignificant(Parser p);
    }
}