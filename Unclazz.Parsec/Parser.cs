using System;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// キャプチャを行わず結果型を持たないパーサーを表す抽象クラスです。
    /// <para>
    /// この抽象クラスから派生するパーサーはパースした値のキャプチャを行わず、パース成否のみを呼び出し元に返します。
    /// キャプチャが必要な場合は<c>Capture()</c>や<c>Map(...)</c>メソッドを呼び出します。
    /// </para>
    /// </summary>
    public abstract class Parser
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parsers.Not(Parser)"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator !(Parser operand)
        {
            return new NotParser(operand.Configuration, operand);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or(Parser, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, Parser right)
        {
            return new OrParser(left.Configuration, left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or(Parser, Parser)"/>と同義です。
        /// 右被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, string right)
        {
            return new OrParser(left.Configuration, left, new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="ParserExtension.Or(Parser, Parser)"/>と同義です。
        /// 左被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(string left, Parser right)
        {
            return new OrParser(new KeywordParser(left), right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then(Parser, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator &(Parser left, Parser right)
        {
            return left.Then(right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then(Parser, Parser)"/>と同義です。
        /// 右被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser operator &(Parser left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="ParserExtension.Then(Parser, Parser)"/>と同義です。
        /// 左被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser operator &(string left, Parser right)
        {
            return new KeywordParser(left).Then(right);
        }
        #endregion

        /// <summary>
        /// デフォルトのコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        protected Parser()
        {
            _factory = ParserFactory.Default;
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }
        /// <summary>
        /// 引数で指定されたコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        /// <param name="config"></param>
        protected Parser(IParserConfiguration config)
        {
            _factory = new ParserFactory(config) ?? throw new ArgumentNullException(nameof(config));
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }

        readonly ParserFactory _factory;
        bool _autoSkip;
        bool _parseLogging;

        /// <summary>
        /// このパーサーのコンフィギュレーションです。
        /// </summary>
        public IParserConfiguration Configuration => _factory;

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ResultCore"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ResultCore"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        protected abstract ResultCore DoParse(Reader input);

        /// <summary>
        /// パース成功を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        protected ResultCore Success()
        {
            return ResultCore.OfSuccess(true);
        }
        /// <summary>
        /// パース成功を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore Success(bool canBacktrack)
        {
            return ResultCore.OfSuccess(canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <returns></returns>
        protected ResultCore Failure(string message)
        {
            return ResultCore.OfFailure(message, true);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore Failure(string message, bool canBacktrack)
        {
            return ResultCore.OfFailure(message, canBacktrack);
        }

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="Result"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="Result"/>を通じて呼び出し元に通知されます。
        /// </para>
        /// <para>
        /// このメソッドは事前処理の後、具象クラスが実装する<see cref="DoParse(Reader)"/>を呼び出します。
        /// その後事後処理を終えてから、呼び出し元に結果を返します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public Result Parse(Reader input)
        {
            if (_autoSkip) SkipWhileIn(input, _factory.SkipTarget);
            var start = input.Position;
            if (_parseLogging)
            {
                LogPreParse(input.Position, input.Peek());
                var res = DoParse(input);
                LogPostParse(input.Position, input.Peek(), res);
                return res.AttachPosition(start, input.Position);
            }
            return DoParse(input).AttachPosition(start, input.Position);
        }
        /// <summary>
        /// このパーサーのコンフィギュレーションを変更します。
        /// </summary>
        /// <param name="act">変更を行うアクション</param>
        public void Configure(Action<IParserConfigurer> act)
        {
            act(_factory);
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }
        void LogPreParse(CharPosition pos, int peek)
        {
            WriteLine("##### Pre-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void LogPostParse(CharPosition pos, int peek, ResultCore result)
        {
            WriteLine("##### Post-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Successful : {0} ", result.Successful);
            if (!result.Successful)
                WriteLine("Message    : {0} ", result.Message);
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void WriteLine(string format, params object[] args)
        {
            _factory.ParseLogger(string.Format(format, args));
        }
        void SkipWhileIn(Reader r, CharClass c)
        {
            while (!r.EndOfFile)
            {
                var ch = (char)r.Peek();
                if (!c.Contains(ch)) break;
                r.Read();
            }
        }
    }
}
