using System;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Nil"/>を読み取り結果型として宣言する<see cref="Parser{T}"/>の特殊な派生型です。
    /// <para>
    /// <see cref="Nil"/>はインスタンスを持ちません。
    /// このパーサー抽象クラスから派生した具象パーサー・クラスは値のキャプチャを一切行いません。
    /// それらのパーサーはパースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、パース結果の成否と関係なく、
    /// <see cref="ParseResult{T}.Capture"/>は必ず空の（値を持たない）インスタンスになります。
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
        public static Parser operator |(Parser left, string right)
        {
            return new OrParser(left.Configuration, left, new KeywordParser(right));
        }
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
        public static Parser operator &(Parser left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        public static Parser operator &(string left, Parser right)
        {
            return new KeywordParser(left).Then(right);
        }
        #endregion

        protected Parser()
        {
            _factory = ParserFactory.Default;
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }
        protected Parser(IParserConfiguration config)
        {
            _factory = new ParserFactory(config) ?? throw new ArgumentNullException(nameof(config));
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }

        readonly ParserFactory _factory;
        bool _autoSkip;
        bool _parseLogging;

        public IParserConfiguration Configuration => _factory;

        protected abstract ResultCore DoParse(Reader input);

        protected ResultCore Success()
        {
            return ResultCore.OfSuccess(true);
        }
        protected ResultCore Success(bool canBacktrack)
        {
            return ResultCore.OfSuccess(canBacktrack);
        }
        protected ResultCore Failure(string message)
        {
            return ResultCore.OfFailure(message, true);
        }
        protected ResultCore Failure(string message, bool canBacktrack)
        {
            return ResultCore.OfFailure(message, canBacktrack);
        }

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
        void LogPreParse(CharacterPosition pos, int peek)
        {
            WriteLine("##### Pre-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void LogPostParse(CharacterPosition pos, int peek, ResultCore result)
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
