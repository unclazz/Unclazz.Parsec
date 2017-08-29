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
        Action<string> ParseLogger { get; }
        /// <summary>
        /// パース処理前後のログ出力を行う場合<c>true</c>。
        /// </summary>
        bool ParseLogging { get; }
        /// <summary>
        /// パース処理上無意味な文字をパース（スキップ）するパーサーです。
        /// </summary>
        CharClass SkipTarget { get; }
        /// <summary>
        /// <see cref="SkipTarget"/>に該当する文字を自動スキップする場合<c>true</c>。
        /// </summary>
        bool AutoSkip { get; }

        /// <summary>
        /// このコンフィギュレーションをコピーします。
        /// </summary>
        /// <returns></returns>
        IParserConfiguration Copy();
    }
}