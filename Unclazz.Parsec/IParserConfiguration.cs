using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーのコンフィギュレーションを表すインターフェースです。
    /// </summary>
    public interface IParserConfiguration
    {
        /// <summary>
        /// パース処理前後のロギングに使用されるロガーです。
        /// </summary>
        Action<string> Logger { get; }
        /// <summary>
        /// パース処理上無意味な文字をパース（スキップ）するパーサーです。
        /// </summary>
        Parser NonSignificant { get; }

        /// <summary>
        /// このコンフィギュレーションをコピーします。
        /// </summary>
        /// <returns></returns>
        IParserConfiguration Copy();
    }
}