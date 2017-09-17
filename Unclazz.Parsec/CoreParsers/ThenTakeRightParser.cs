using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 2つのパーサーを連結しシーケンスを構成するパーサーです。
    /// 結果値は右被演算子（引数）側のパーサーが返すものとなります。
    /// </summary>
    /// <typeparam name="TRight">右被演算子（引数）側の結果値の型</typeparam>
    sealed class ThenTakeRightParser<TRight> : Parser<TRight>
    {
        internal ThenTakeRightParser(Parser left, Parser<TRight> right) :base("Then")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public Parser Left { get; }
        public Parser<TRight> Right { get; }
        protected override ResultCore<TRight> DoParse(Context ctx)
        {
            // 左被演算子側のパーサーでパース
            var leftResult = Left.Parse(ctx);

            // パース結果をチェック、失敗の場合はその結果を呼び出し元に返す
            // ＊このとき型の変換（付与）を行っているが、
            // 失敗を表すオブジェクトのためキャプチャ結果として空の値が設定されるリスクはない
            if (!leftResult.Successful) return leftResult.Typed<TRight>();

            // 続いて、右被演算子側のパーサーでパース
            var rightResult = Right.Parse(ctx);

            // 成否に関わらず左右のバックトラック設定を合成
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

            // バックトラック設定のみ変更した右側の結果を返す
            return rightResult.AllowBacktrack(canBacktrack);
        }
    }
}
