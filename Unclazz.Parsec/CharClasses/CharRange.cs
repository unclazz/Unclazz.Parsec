using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    struct CharRange
    {
        public static CharRange Between(char start, char end)
        {
            return new CharRange(start, end);
        }
        public static CharRange Exactly(char ch)
        {
            return new CharRange(ch, ch);
        }

        CharRange(char start, char end)
        {
            _hasDelta = start != end;
            Start = start < end ? start : end;
            End = start < end ? end : start;
        }

        bool _hasDelta;

        public char Start { get; }
        public char End { get; }

        public bool Contains(int ch)
        {
            return _hasDelta ? (Start <= ch && ch <= End) : Start == ch;
        }
        public override string ToString()
        {
            if (_hasDelta)
            {
                return string.Format("{0} to {1}",
                    ParsecUtility.CharToString(Start),
                    ParsecUtility.CharToString(End));
            }
            else
            {
                return ParsecUtility.CharToString(Start);
            }
        }
    }
    static class CharRangeUtility
    {
        public static CharRange[] AnyOf(params char[] cs)
        {
            return AnyOf((IEnumerable<char>)cs);
        }
        public static CharRange[] AnyOf(IEnumerable<char> cs)
        {
            if (cs == null) throw new ArgumentNullException(nameof(cs));
            var leftSeq = cs.Distinct().OrderBy(a => a).ToArray();
            if (leftSeq.Length == 0) return new CharRange[0];
            if (leftSeq.Length == 1) return new CharRange[] { CharRange.Exactly(leftSeq[0]) };

            // シーケンスの先頭の文字を重複させたシーケンスを作成
            var rightSeq = new char[] { leftSeq[0] }.Concat(leftSeq);

            // 元のシーケンスと新しいシーケンスをZIPして匿名型のシーケンスを作成。
            // Content:文字クラスを構成する文字、Prev:それらの文字の1つ前の文字。
            var zip = leftSeq.Zip(rightSeq, (left, right) => new {
                Content = left,
                PrevContent = right
            });

            // コード差分とコード自体を内容とする匿名型のシーケンスに変換。
            var increments = zip.Select(chx => new {
                Increment = chx.Content - chx.PrevContent,
                Content = chx.Content
            });

            // 文字の範囲を構成するメンバを一時的に格納するバッファ
            var charsBuff = new List<char>();
            // 文字の範囲を表すオブジェクトを一時的に格納するバッファ
            var rangesBuff = new List<CharRange>();

            // コード差分とコード自体を内容とする匿名型のシーケンスをループ処理
            foreach (var pair in increments)
            {
                // コード差分が1より大きい＝文字の範囲の分断が生じる
                if (pair.Increment > 1)
                {
                    // 範囲メンバを格納しているバッファの内容をもとに範囲オブジェクトを生成
                    // 範囲オブジェクト用のバッファに追加
                    rangesBuff.Add(CharRange.Between(charsBuff.First(), charsBuff.Last()));
                    // 範囲メンバを格納しているバッファはクリア
                    charsBuff.Clear();
                }
                // 文字を文字範囲メンバ用のバッファに追加
                charsBuff.Add(pair.Content);
            }

            // ループを抜けたあと範囲化の済んでいないメンバがいないかチェック
            if (charsBuff.Count > 0)
            {
                // 残っていたメンバの情報から範囲オブジェクトを作成しバッファに追加
                rangesBuff.Add(CharRange.Between(charsBuff.First(), charsBuff.Last()));
            }

            // 範囲オブジェクトのシーケンスを呼び出し元に返す
            return rangesBuff.ToArray();
        }
        public static CharRange[] TryAppend(CharRange r, char ch)
        {
            return r.Contains(ch) ? new[] { r } : TryMerge(r, CharRange.Exactly(ch));
        }
        public static CharRange[] TryMerge(params CharRange[] rs)
        {
            return TryMerge((IEnumerable<CharRange>)rs);
        }
        public static CharRange[] TryMerge(IEnumerable<CharRange> rs)
        {
            if (rs == null) throw new ArgumentNullException(nameof(rs));
            return rs.OrderBy(r => r.Start).Aggregate(new Stack<CharRange>(), TryMerge_Accumulate).ToArray();
        }
        static Stack<CharRange> TryMerge_Accumulate(Stack<CharRange> stack, CharRange range)
        {
            if (stack.Count == 0)
            {
                stack.Push(range);
                return stack;
            }
            var res = TryMerge_Accumulate_Merge2Range(stack.Peek(), range);
            if (res.Present)
            {
                stack.Pop();
                stack.Push(res.Value);
            }
            else
            {
                stack.Push(range);
            }
            return stack;
        }
        static Optional<CharRange> TryMerge_Accumulate_Merge2Range(CharRange left, CharRange right)
        {
            if (/* left.Start <= right.Start && */ right.Start <= left.End)
            {
                if (left.End <= right.End || (right.End - left.End) == 1)
                {
                    return new Optional<CharRange>(CharRange.Between(left.Start, right.End));
                }
                return new Optional<CharRange>(left);
            }
            return new Optional<CharRange>();
        }
    }
}
