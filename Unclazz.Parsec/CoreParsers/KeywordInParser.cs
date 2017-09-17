using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 指定されたキーワードのいずれかにマッチするパーサーです。
    /// キーワードのコレクションは内部的に辞書順ソートされ一意性確保もされます。
    /// ソート後のコレクションで隣接する要素同士が共通する接頭辞が検出された場合、
    /// それらの文字列の直後にカット（Cut）が設定されます。
    /// </summary>
    sealed class KeywordInParser : Parser
    {
        /// <summary>
        /// 2つのキーワードの共通の接頭辞の文字数を算出します。
        /// </summary>
        /// <param name="keyword0"></param>
        /// <param name="keyword1"></param>
        /// <returns></returns>
        static int CommonPrefixLength(string keyword0, string keyword1)
        {
            // 2つのキーワードをcharシーケンスとしてZIP。
            // 同一位置の文字同士を先頭から順番に比較した結果からなるboolシーケンスを得、
            // そのシーケンスの先頭から連続しているtrueの件数をカウントする。
            return keyword0.Zip(keyword1,
                    (prevChar, currChar) => prevChar == currChar)
                    .TakeWhile(a => a).Count();
        }
        /// <summary>
        /// キーワードのコレクションからそれらにマッチするパーサーを組み立てます。
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        static Parser KeywordsParser(string[] keywords)
        {
            // キーワード数が1の場合、単にKeywordParserのインスタンスを返すだけ
            if (keywords.Length == 1) return new KeywordParser(keywords[0]);

            // 合成過程のパーサーを格納する一時変数
            Parser tmp = null;
            
            // キーワードのコレクションをそれ自身（ただし最初の要素はスキップ）とZIPして、
            // {キーワード, 隣接するキーワードとの共通接頭辞の長さ}
            // という2メンバーを持つ一時型インスタンスのシーケンスに変換
            var zip = keywords.Zip(keywords.Skip(1), (k0, k1) =>
            new { Keyword = k0, CommonPrefixLength = CommonPrefixLength(k0, k1) });

            // ZIPした結果に基づきループを行いパーサーを段階構築
            foreach (var zipElem in zip)
            {
                // キーワードの長さ と 共通接頭辞の長さ＋1 のいずれか小さい方をカット（Cut）の文字位置として採用
                // ＊共通接頭辞の長さに＋1をすると、キーワードの長さをオーバーする可能性がある。
                // これはKeywordParserのコンストラクタ引数の値として不正とみなされるので、
                // キーワード自体の長さと比較を行い短い方を採用する。
                var cutIndex = Math.Min(zipElem.Keyword.Length, zipElem.CommonPrefixLength + 1);
                
                // キーワードを単体で読み取るパーサーを作成
                var nextParser = new KeywordParser(zipElem.Keyword, cutIndex);
                
                // 一時変数が空 ＝ 初回 は、作成したパーサーを単純にアサインする
                // 一時変数が空でない ＝ 2回目以降 は、Orで連結して再アサインする
                tmp = tmp == null ? nextParser : tmp.Or(nextParser);
            }

            // 終わりに、キーワードのコレクションの末尾のものをOrで連結する
            // ＊ZIPを行った時、末尾のキーワードは除外されてしまうため、ここで救済する
            return tmp.Or(new KeywordParser(keywords[keywords.Length - 1]));
        }

        internal KeywordInParser(string[] keywords) : base("KeywordIn")
        {
            // キーワードのコレクションはnullであってはならない
            var tmp = keywords ?? throw new ArgumentNullException(nameof(keywords));
            // キーワードのコレクションは1つ以上の要素を持つこと
            if (tmp.Length == 0) throw new ArgumentException(nameof(keywords) + " must not be empty.");
            // キーワードのコレクションはnullを含まないこと
            if (tmp.Any(k => k == null)) throw new ArgumentException(nameof(keywords) + " must not contain null.");
            // キーワードのコレクションは""を含まないこと
            if (tmp.Any(k => k.Length == 0)) throw new ArgumentException(nameof(keywords) + " must not contain empty string.");
            // キーワードのコレクションをソート＆一意性確保
            _keywords = tmp.Distinct().OrderBy(k => k).ToArray();
            // 内部パーサーを構築
            _parser = KeywordsParser(tmp);

        }
        readonly string[] _keywords;
        readonly Parser _parser;
        protected override ResultCore DoParse(Context ctx)
        {
            return _parser.Parse(ctx);
        }
    }
}
