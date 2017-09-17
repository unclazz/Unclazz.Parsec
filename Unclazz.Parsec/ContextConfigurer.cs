using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Context"/>のコンフィギュレーションを変更するためのクラスです。
    /// </summary>
    public sealed class ContextConfigurer
    {
        internal ContextConfigurer(Context orig)
        {
            _orig = orig ?? throw new ArgumentNullException(nameof(orig));
            _skipTarget = orig.SkipTarget;
            _logAppender = orig.LogAppender;
        }

        readonly Context _orig;
        CharClass _skipTarget;
        Action<string> _logAppender;

        /// <summary>
        /// ログ出力で使用されるアペンダーを設定します。
        /// <para>ログを無効化する場合は<c>null</c>を指定します。</para>
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public ContextConfigurer SetLogAppender(Action<string> act)
        {
            _logAppender = act;
            return this;
        }
        /// <summary>
        /// 自動スキップの対象となる文字クラスを設定します。
        /// <para>自動スキップを無効化する場合は<c>null</c>を指定します。</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
        public ContextConfigurer SetSkipTarget(CharClass clazz)
        {
            _skipTarget = clazz;
            return this;
        }
        internal Context Make()
        {
            return new Context(_orig.Source, _orig.Stack, _logAppender, _skipTarget);
        }
    }
}
