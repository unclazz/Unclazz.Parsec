using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パース処理のコンテキストを表すクラスです。
    /// <para>パーサー開発者に対して入力データソースへのアクセスやロギングAPIを提供します。</para>
    /// <para>このクラスのインスタンスは<see cref="string"/>や<see cref="Reader"/>からの暗黙キャストのほか、
    /// <see cref="Reader.ToContext()"/>とその多重定義を通じても取得できます。</para>
    /// </summary>
    public sealed class Context
    {
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="text"></param>
        public static implicit operator Context(string text)
        {
            return new Context(Reader.From(text));
        }
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Context(Reader source)
        {
            return new Context(source);
        }

        internal Context(Reader source) : this(source, null, null, null) { }
        internal Context(Reader source, Stack<string> stack, Action<string> logAppender, CharClass skipTarget)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Stack = stack ?? (logAppender == null ? null : new Stack<string>());
            LogAppender = logAppender;
            SkipTarget = skipTarget;
            AutoSkip = skipTarget != null;
            Logging = logAppender != null;
        }

        /// <summary>
        /// 入力データソースです。
        /// </summary>
        public Reader Source { get; }
        /// <summary>
        /// ロギングが有効な場合<c>true</c>です。
        /// </summary>
        public bool Logging { get; }
        /// <summary>
        /// 自動スキップが有効な場合<c>true</c>です。
        /// </summary>
        public bool AutoSkip { get; }
        /// <summary>
        /// ロギングで使用されるアペンダーです。
        /// 未設定の場合（ロギングが無効である場合）、<c>null</c>が返されます。
        /// </summary>
        internal Action<string> LogAppender { get; }
        /// <summary>
        /// 自動スキップで使用されるアペンダーです。
        /// 未設定の場合（自動スキップが無効である場合）、<c>null</c>が返されます。
        /// </summary>
        internal CharClass SkipTarget { get; }
        /// <summary>
        /// パーサーの呼び出しスタックです。
        /// </summary>
        internal Stack<string> Stack { get; }

        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合は文字列を作成してアペンダーをコール
            LogAppender(MakeLabel(' ').Append(message).ToString());
        }
        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public void Log(string format, object arg0)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0));
        }
        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public void Log(string format, object arg0, object arg1)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0, arg1));
        }
        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Log(string format, object arg0, object arg1, object arg2)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0, arg1, arg2));
        }
        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Log(string format, params object[] args)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, args));
        }
        /// <summary>
        /// コンテキストのコンフィギュレーションを変更します。
        /// <para>
        /// このメソッドは新しいコンテキストのインスタンスを生成して返します。
        /// もとのインスタンスは変更されません。
        /// </para>
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public Context Configure(Action<ContextConfigurer> act)
        {
            var config = new ContextConfigurer(this);
            act(config);
            return config.Make();
        }
        /// <summary>
        /// パース本処理前に呼び出しスタックを更新し、自動スキップとロギングを行います。
        /// </summary>
        /// <param name="parserName"></param>
        internal void PreParse(string parserName)
        {
            // 自動スキップを試行
            DoSkip();
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // スタックにパーサー名を追加
            Stack.Push(parserName);
            // パース開始を示すログメッセージを作成してアペンダーをコール
            LogAppender(MakeLabel('+').ToString());
        }
        /// <summary>
        /// 自動スキップを行います。
        /// </summary>
        internal void DoSkip()
        {
            // スキップ対象が存在しない場合は直ちに処理を終了
            if (SkipTarget == null) return;
            // 存在する場合はスキップを実行
            while (!Source.EndOfFile)
            {
                var ch = (char)Source.Peek();
                if (!SkipTarget.Contains(ch)) break;
                Source.Read();
            }
        }
        /// <summary>
        /// パース本処理後にロギングを行い、呼び出しスタックを更新します。
        /// </summary>
        /// <param name="result"></param>
        internal void PostParse(ResultCore result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-');
            if (result.Successful) buff.Append("Success(");
            else buff.Append("Failure(").Append(result.Message);
            if (!result.CanBacktrack) buff.Append(", cut");
            LogAppender(buff.Append(')').ToString());

            // スタックから直近のパーサー名を削除
            Stack.Pop();
        }
        /// <summary>
        /// パース本処理後にロギングを行い、呼び出しスタックを更新します。
        /// </summary>
        /// <param name="result"></param>
        internal void PostParse<T>(ResultCore<T> result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-');
            if (result.Successful) buff.Append("Success(").Append(result.Capture);
            else buff.Append("Failure(").Append(result.Message);
            if (!result.CanBacktrack) buff.Append(", cut");
            LogAppender(buff.Append(')').ToString());

            // スタックから直近のパーサー名を削除
            Stack.Pop();
        }
        StringBuilder MakeLabel(char sign)
        {
            var pos = Source.Position;
            return new StringBuilder()
                .Append(' ', (Stack.Count -1) * 2)
                .Append(sign).Append(' ').Append(Stack.Peek())
                .Append(" (ln=").Append(pos.Line).Append(", col=")
                .Append(pos.Column).Append(", idx=").Append(pos.Index).Append(") ");
        }
    }
}
