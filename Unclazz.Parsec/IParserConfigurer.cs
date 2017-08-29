using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーのコンフィギュレーションを変更するためのインターフェースです。
    /// </summary>
    public interface IParserConfigurer
    {
        /// <summary>
        /// <see cref="IParserConfiguration.ParseLogger"/>を設定します。
        /// </summary>
        /// <param name="logger">ロガーとして使用されるアクション</param>
        /// <returns></returns>
        IParserConfigurer SetParseLogger(Action<string> logger);
        /// <summary>
        /// <see cref="IParserConfiguration.ParseLogging"/>を設定します。
        /// </summary>
        /// <param name="onOff">ログを有効化する場合は<c>true</c></param>
        /// <returns></returns>
        IParserConfigurer SetParseLogging(bool onOff);
        /// <summary>
        /// <see cref="IParserConfiguration.SkipTarget"/>を設定します。
        /// </summary>
        /// <param name="clazz">スキップ対象の文字クラス</param>
        /// <returns></returns>
        IParserConfigurer SetSkipTarget(CharClass clazz);
        /// <summary>
        /// <see cref="IParserConfiguration.AutoSkip"/>を設定します。
        /// </summary>
        /// <param name="onOff">スキップを有効化する場合は<c>true</c></param>
        /// <returns></returns>
        IParserConfigurer SetAutoSkip(bool onOff);
    }
}