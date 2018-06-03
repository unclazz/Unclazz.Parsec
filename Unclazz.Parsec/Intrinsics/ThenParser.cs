﻿using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class ThenParser : Parser
    {
        internal ThenParser(Parser left, Parser right) : base("Then")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public Parser Left { get; }
        public Parser Right { get; }
        protected override ResultCore DoParse(Reader src)
        {
            // 左側のパーサーでパース
            var leftResult = Left.Parse(src);

            // 結果NGの場合、ただちにその結果を呼び出し元に帰す
            if (!leftResult.Successful) return leftResult;

            // 右側のパーサーでパース
            var rightResult = Right.Parse(src);

            // バックトラック設定を合成
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

            // 右側の結果を、バックトラック設定のみカスタマイズし、呼び出し元に返す
            return rightResult.AllowBacktrack(canBacktrack);
        }
    }
}
