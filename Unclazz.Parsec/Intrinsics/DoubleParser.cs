using System;

namespace Unclazz.Parsec.Intrinsics
{
    /// <summary>
    /// 2つのパーサーを連結しシーケンスを構成するパーサーです。
    /// 結果値は両側のパーサーが返す結果をタプルでまとめたものとなります。
    /// </summary>
    /// <typeparam name="T1">左被演算子（レシーバー）側の結果型</typeparam>
    /// <typeparam name="T2">右被演算子（引数）側の結果型</typeparam>
    sealed class DoubleParser<T1, T2> : Parser<Tuple<T1, T2>>
    {
        internal DoubleParser(Parser<T1> left, Parser<T2> right) : base("Double")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<T1> Left { get; }
        public Parser<T2> Right { get; }

        protected override ResultCore<Tuple<T1, T2>> DoParse(Reader src)
        {
            // 左側のパーサーでパース
            var leftResult = Left.Parse(src);
            
            // 結果NGの場合、その結果を呼び出し元に返す
            if (!leftResult.Successful) return Failure(leftResult.Message);

            // 右側のパーサーでパース
            var rightResult = Right.Parse(src);

            // 成否に関わらず左右のバックトラック設定を合成
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

            // 結果NGの場合、その結果を呼び出し元に返す
            if (!rightResult.Successful) return Failure(rightResult.Message, canBacktrack);

            // 両側成功の場合、それぞれの結果をタプルにまとめて呼び出し元に返す
            return Success(new Tuple<T1, T2>(leftResult.Capture, rightResult.Capture), canBacktrack);
        }
    }
}
