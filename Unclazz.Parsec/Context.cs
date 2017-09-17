using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    public sealed class Context
    {
        public static implicit operator Context(string text)
        {
            return new Context(Reader.From(text));
        }
        public static implicit operator Context(Reader source)
        {
            return new Context(source);
        }

        public Context(Reader source) : this(source, null, null, null) { }
        internal Context(Reader source, Stack<string> stack, Action<string> logAppender, CharClass skipTarget)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Stack = stack ?? (logAppender == null ? null : new Stack<string>());
            LogAppender = logAppender;
            SkipTarget = skipTarget;
            AutoSkip = skipTarget != null;
            Logging = logAppender != null;
        }

        public Reader Source { get; }
        public bool Logging { get; }
        public bool AutoSkip { get; }
        internal Action<string> LogAppender { get; }
        internal CharClass SkipTarget { get; }
        internal Stack<string> Stack { get; }

        public void Log(string message)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合は文字列を作成してアペンダーをコール
            LogAppender(MakeLabel(' ').Append(message).ToString());
        }
        public void Log(string format, object arg0)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0));
        }
        public void Log(string format, object arg0, object arg1)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0, arg1));
        }
        public void Log(string format, object arg0, object arg1, object arg2)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, arg0, arg1, arg2));
        }
        public void Log(string format, params object[] args)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;
            // 存在する場合はメッセージを作成してLog(string)をコール
            Log(string.Format(format, args));
        }
        public Context Configure(Action<ContextConfigurer> ctxConfig)
        {
            var config = new ContextConfigurer(this);
            ctxConfig(config);
            return config.Make();
        }
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
        internal void PostParse(ResultCore result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-').Append(result.Successful ? "Success(" : "Failure(");
            if (result.Successful) buff.Append("Success(");
            else buff.Append("Failure(").Append(result.Message);
            if (!result.CanBacktrack) buff.Append(", cut");
            LogAppender(buff.Append(')').ToString());

            // スタックから直近のパーサー名を削除
            Stack.Pop();
        }
        internal void PostParse<T>(ResultCore<T> result)
        {
            // アペンダーが存在しない場合は処理を直ちに終了
            if (LogAppender == null) return;

            // パース終了を示すログメッセージを作成してアペンダーをコール
            var buff = MakeLabel('-').Append(result.Successful ? "Success(" : "Failure(");
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
                .Append(' ', Stack.Count * 2)
                .Append(sign).Append(' ').Append(Stack.Peek())
                .Append(" (ln=").Append(pos.Line).Append(", col=")
                .Append(pos.Column).Append(", idx=").Append(pos.Index).Append(") ");
        }
    }

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

        public ContextConfigurer SetLogAppender(Action<string> act)
        {
            _logAppender = act;
            return this;
        }
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
