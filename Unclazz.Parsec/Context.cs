using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パース処理のコンテキストを表すクラスです。
    /// <para>パーサー開発者に対してパーサー呼び出し階層へのアクセスやロギング機能を提供します。</para>
    /// </summary>
    public sealed class Context
    {
        internal Context(Reader source, bool callStack, Action<string> logAppender)
        {
            _reader = source ?? throw new ArgumentNullException(nameof(source));
            _nolog = logAppender == null;
            _nostack = !callStack && _nolog;
            _stack = _nostack ? null : new Stack<ParseCall>();
            _logAppender = logAppender;
        }

        readonly Action<string> _logAppender;
        readonly Stack<ParseCall> _stack;
        readonly bool _nolog;
        readonly bool _nostack;
        readonly Reader _reader;

        /// <summary>
        /// 呼び出し階層です。
        /// </summary>
        /// <value></value>
        public IEnumerable<ParseCall> CallStack => _stack?.ToArray();
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
            // 呼び出し階層の記録がOFFの場合は処理を直ちに終了
            if (_nostack) return;
            // スタックにパーサー名を追加
            _stack.Push(new ParseCall(parserName, _reader.Position, _stack.Count + 1));

            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) return;
            // パース開始を示すログメッセージを作成してアペンダーをコール
            _logAppender(MakeLabel('+').ToString());
        }
        /// <summary>
        /// パース本処理後にロギングを行い、呼び出しスタックを更新します。
        /// </summary>
        /// <param name="result"></param>
        internal void PostParse(ResultCore result)
        {
            // 呼び出し階層の記録がOFFの場合は処理を直ちに終了
            if (_nostack) return;

            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog) 
            {
                _stack?.Pop();
                return;
            }

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
            // 呼び出し階層の記録がOFFの場合は処理を直ちに終了
            if (_nostack) return;

            // アペンダーが存在しない場合は処理を直ちに終了
            if (_nolog)
            {
                _stack?.Pop();
                return;
            }

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
            var pos = _reader.Position;
            return new StringBuilder()
                .Append(' ', (_stack.Count - 1) * 2)
                .Append(sign).Append(' ').Append(_stack.Peek().ParserName)
                .Append(" (ln=").Append(pos.Line).Append(", col=")
                .Append(pos.Column).Append(", idx=").Append(pos.Index).Append(") ");
        }
    }
}
