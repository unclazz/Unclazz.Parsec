using System;

namespace Unclazz.Parsec
{
    public interface IParserConfigurer
    {
        IParserConfigurer SetLogger(Action<string> l);
        IParserConfigurer SetNonSignificant(Parser p);
    }
}