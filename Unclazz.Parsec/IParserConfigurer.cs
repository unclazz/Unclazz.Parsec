using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーのコンフィギュレーションを変更するためのインターフェースです。
    /// </summary>
    public interface IParserConfigurer
    {
        /// <summary>
        /// <see cref="IParserConfiguration.Logger"/>を設定します。
        /// </summary>
        /// <param name="l">ロガーとして使用されるアクション</param>
        /// <returns></returns>
        IParserConfigurer SetLogger(Action<string> l);
        /// <summary>
        /// <see cref="IParserConfiguration.NonSignificant"/>を設定します。
        /// </summary>
        /// <param name="p">パース（スキップ）のために使用されるパーサー</param>
        /// <returns></returns>
        IParserConfigurer SetNonSignificant(Parser p);
    }
}