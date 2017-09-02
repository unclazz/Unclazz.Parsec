using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 2つのパーサーを連結しシーケンスを構成するパーサーです。
    /// 結果値は左被演算子（レシーバー）側のパーサーが返すものとなります。
    /// </summary>
    /// <typeparam name="TLeft">左被演算子（レシーバー）側の結果値の型</typeparam>
    sealed class ThenTakeLeftParser<TLeft> : Parser<TLeft>
    {
        internal ThenTakeLeftParser(IParserConfiguration conf, Parser<TLeft> left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public Parser<TLeft> Left { get; }
        public Parser Right { get; }
        protected override ResultCore<TLeft> DoParse(Reader input)
        {
            // 左被演算子側のパーサーでパース
            var leftResult = Left.Parse(input);

            // パース結果をチェック、失敗の場合はその結果を呼び出し元に返す
            if (!leftResult.Successful) return leftResult;

            // 続いて、右被演算子側のパーサーでパース
            var rightResult = Right.Parse(input);
            
            // 成否に関わらず左右のバックトラック設定を合成
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
            
            // パース結果をチェック
            if (rightResult.Successful)
            {
                // 成功の場合、バックトラック設定のみ変更した左側の結果を返す
                return leftResult.AllowBacktrack(canBacktrack);
            }
            else
            {
                // 失敗の場合、右側結果ベースで結果を生成して返す
                return Failure(rightResult.Message, canBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
