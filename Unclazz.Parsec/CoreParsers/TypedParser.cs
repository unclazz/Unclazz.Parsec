using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 結果型を持たないパーサーに結果型を付与するパーサーです。
    /// コンストラクタに結果値として使用される値を指定しなかった場合、
    /// パーサーが返す結果値は結果型<typeparamref name="TResult"/>のデフォルト値となります。
    /// </summary>
    /// <typeparam name="TResult">このパーサーの結果型</typeparam>
    sealed class TypedParser<TResult> : Parser<TResult>
    {
        internal TypedParser(Parser original) : this(original, default(TResult)) { }
        internal TypedParser(Parser original, TResult value) : base(original.Configuration)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _value = value;
        }
        readonly Parser _original;
        readonly TResult _value;
        protected override ResultCore<TResult> DoParse(Reader input)
        {
            // 元になったパーサーでパースを実施、
            // 結果型の変換のみ行って呼び出し元に返す
            return _original.Parse(input).Typed(_value);
        }
        public override string ToString()
        {
            return _original.ToString();
        }
    }
}
