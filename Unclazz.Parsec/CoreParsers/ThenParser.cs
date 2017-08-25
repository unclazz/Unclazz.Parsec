using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 2つのパーサーを連結するパーサーです。
    /// パーサーの結果型は元になったパーサーのうち左側のそれが採用されます。
    /// </summary>
    /// <typeparam name="L">元のパーサーの結果型（左側）</typeparam>
    /// <typeparam name="R">元のパーサーの結果型（右側）</typeparam>
    sealed class ThenTakeLeftParser<L, R> : Parser<L>
    {
        internal ThenTakeLeftParser(IParser<L> left, IParser<R> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IParser<L> Left { get; }
        public IParser<R> Right { get; }

        public override ParseResult<L> Parse(ParserInput input)
        {
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var p = input.Position;
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                if (rightResult.Successful)
                {
                    return leftResult.AllowBacktrack(canBacktrack);
                }
                else
                {
                    return Failure(p, rightResult.Message, canBacktrack);
                }
            }
            return leftResult;
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
    /// <summary>
    /// 2つのパーサーを連結するパーサーです。
    /// パーサーの結果型は元になったパーサーのうち右側のそれが採用されます。
    /// </summary>
    /// <typeparam name="L">元のパーサーの結果型（左側）</typeparam>
    /// <typeparam name="R">元のパーサーの結果型（右側）</typeparam>
    sealed class ThenTakeRightParser<L, R> : Parser<R>
    {
        internal ThenTakeRightParser(IParser<L> left, IParser<R> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IParser<L> Left { get; }
        public IParser<R> Right { get; }

        public override ParseResult<R> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                return rightResult.AllowBacktrack(canBacktrack);
            }
            return leftResult.Cast<R>();
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
