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
    /// <see cref="Context.Create(Reader)"/>とその多重定義を通じても取得できます。</para>
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
        /// <summary>
        /// 新しいインスタンスを生成して返します。
        /// </summary>
        /// <returns>新しいインスタンス</returns>
        /// <param name="src">入力データ</param>
        /// <exception cref="ArgumentNullException">引数が<c>null</c>である場合</exception>
        public static Context Create(Reader src)
        {
            return new Context(src);
        }
        /// <summary>
        /// 新しいインスタンスを生成して返します。
        /// </summary>
        /// <returns>新しいインスタンス</returns>
        /// <param name="src">入力データ</param>
        /// <param name="logAppender">ログ出力に使用されるアクション</param>
        /// <exception cref="ArgumentNullException">引数のいずれかが<c>null</c>である場合</exception>
        public static Context Create(Reader src, Action<string> logAppender)
        {
            return new Context(src, logAppender ?? throw new ArgumentNullException(nameof(logAppender)));
        }

        internal Context(Reader source) : this(source, null) { }
        internal Context(Reader source, Action<string> logAppender)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _stack = logAppender == null ? null : new Stack<string>();
            _logAppender = logAppender;
            _nolog = logAppender == null;
        }

        readonly Action<string> _logAppender;
        readonly Stack<string> _stack;
        readonly bool _nolog;

        /// <summary>
        /// 入力データソースです。
        /// </summary>
        public Reader Source { get; }
        /// <summary>
        /// ロギングが有効な場合<c>true</c>です。
        /// </summary>
        public bool Logging => _logAppender != null;

        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;
            // 存在する場合は文字列を作成してアペンダーをコール
            _logAppender(MakeLabel(' ').Append(message).ToString());
        }
        /// <summary>
        /// ログを書き出します。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public void Log(string format, object arg0)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;
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
            if (_nolog) return;
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
            if (_nolog) return;
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
            if (_nolog) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, args));
        }
        /// <summary>
        /// パース本処理前に呼び出しスタックを更新し、自動スキップとロギングを行います。
        /// </summary>
        /// <param name="parserName"></param>
        internal void PreParse(string parserName)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;
            // スタックにパーサー名を追加
            _stack.Push(parserName);
            // パース開始を示すログメッセージを作成してアペンダーをコール
            _logAppender(MakeLabel('+').ToString());
        }
        /// <summary>
        /// パース本処理後にロギングを行い、呼び出しスタックを更新します。
        /// </summary>
        /// <param name="result"></param>
        internal void PostParse(ResultCore result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-');
            if (result.Successful) buff.Append("Success(");
            else buff.Append("Failure(").Append(result.Message);
            if (!result.CanBacktrack) buff.Append(", cut");
            _logAppender(buff.Append(')').ToString());

            // スタックから直近のパーサー名を削除
            _stack.Pop();
        }
        /// <summary>
        /// パース本処理後にロギングを行い、呼び出しスタックを更新します。
        /// </summary>
        /// <param name="result"></param>
        internal void PostParse<T>(ResultCore<T> result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-');
            if (result.Successful) buff.Append("Success(").Append(result.Capture);
            else buff.Append("Failure(").Append(result.Message);
            if (!result.CanBacktrack) buff.Append(", cut");
            _logAppender(buff.Append(')').ToString());

            // スタックから直近のパーサー名を削除
            _stack.Pop();
        }
        StringBuilder MakeLabel(char sign)
        {
            var pos = Source.Position;
            return new StringBuilder()
                .Append(' ', (_stack.Count -1) * 2)
                .Append(sign).Append(' ').Append(_stack.Peek())
                .Append(" (ln=").Append(pos.Line).Append(", col=")
                .Append(pos.Column).Append(", idx=").Append(pos.Index).Append(") ");
        }
    }

}
