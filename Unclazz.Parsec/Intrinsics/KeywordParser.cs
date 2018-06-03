using System;
using System.Collections.Generic;
using System.Text;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class KeywordParser : Parser
    {
        internal KeywordParser(string keyword) : this(keyword, -1) { }
        internal KeywordParser(string keyword, int cutIndex) : base("Keyword")
        {
            // カットをONにする文字位置（文字境界）は-1以上 かつ 0以外であること
            if (cutIndex < -1 || cutIndex == 0) throw new ArgumentOutOfRangeException(nameof(cutIndex));
            // キーワードはnullでないこと
            if (keyword == null) throw new ArgumentNullException(nameof(keyword));
            // キーワード文字数は1文字以上であること
            if (keyword.Length == 0) throw new ArgumentException("length of keyword is must be greater than 0.");
            // キーワード文字数よりも大きなカット位置は無効
            if (keyword.Length < cutIndex) throw new ArgumentOutOfRangeException(nameof(cutIndex));
            // キーワード文字数とカット位置が同一の場合カットはOFF、それ以外の場合指定された位置でカット
            _cut = keyword.Length == cutIndex ? -1 : cutIndex;
            _keyword = keyword;
        }

        readonly int _cut;
        readonly string _keyword;

        protected override ResultCore DoParse(Reader src)
        {
            // キーワードの左側から順番に文字の照合を実施
            // ＊照合中にEOFに到達した場合も通常の文字と同じ比較処理で検出し
            // ハンドリングできるため、ループ条件部には何もチェックを入れない
            for (var i = 0; i < _keyword.Length; i++)
            {
                // 期待される文字
                var expected = _keyword[i];
                // 実際の文字
                var actual = src.Read();
                // 比較を行う
                if (expected != actual)
                {
                    // NGの場合、その旨の結果を呼び出し元に返す
                    return Failure(string.Format("expected {0} but found {1} at index {2} in \"{3}\"",
                        ParsecUtility.CharToString(expected), ParsecUtility.CharToString(actual), i, _keyword),
                        -1 == _cut || i < _cut);
                }
            }
            return Success();
        }
    }
}
