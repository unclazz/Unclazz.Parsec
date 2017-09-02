using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 2つのパーサーを連結しシーケンスを構成するパーサーです。
    /// 両側のパーサーが結果型を持たないパーサーなので、それを合成したこのパーサーも結果型を持ちません。
    /// </summary>
    sealed class ThenParser : Parser
    {
        internal ThenParser(IParserConfiguration conf, Parser left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public Parser Left { get; }
        public Parser Right { get; }
        protected override ResultCore DoParse(Reader input)
        {
            // 左側のパーサーでパース
            var leftResult = Left.Parse(input);

            // 結果NGの場合、ただちにその結果を呼び出し元に帰す
            if (!leftResult.Successful) return leftResult;

            // 右側のパーサーでパース
            var rightResult = Right.Parse(input);

            // バックトラック設定を合成
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

            // 右側の結果を、バックトラック設定のみカスタマイズし、呼び出し元に返す
            return rightResult.AllowBacktrack(canBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
