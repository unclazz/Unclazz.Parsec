using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 結果型を持つパーサーから結果型を取り除くパーサーです。
    /// 元になったパーサーのキャプチャ結果は破棄されます。
    /// </summary>
    /// <typeparam name="TSource">元になったパーサーの結果型</typeparam>
    sealed class UntypedParser<TSource> : Parser
    {
        internal UntypedParser(Parser<TSource> original) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser<TSource> _original;
        protected override ResultCore DoParse(Reader input)
        {
            // 元になったパーサーでパース、
            // 結果型の変更（剥奪）のみ行って呼び出し元に返す
            return _original.Parse(input).Untyped();
        }
        public override string ToString()
        {
            return _original.ToString();
        }
    }
}
