using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClass
{
    /// <summary>
    /// 文字の範囲を表すクラスです。
    /// </summary>
    public sealed class CharRange
    {
        /// <summary>
        /// このクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="start">始点</param>
        /// <param name="end">終点</param>
        /// <returns>インスタンス</returns>
        public static CharRange Between(char start, char end)
        {
            return start <= end ? new CharRange(start, end) : new CharRange(end, start);
        }

        CharRange(char start, char end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// 範囲の始点です。
        /// </summary>
        public char Start { get; }
        /// <summary>
        /// 範囲の終点です。
        /// </summary>
        public char End { get; }
        /// <summary>
        /// 文字が範囲に含まれるかどうかをチェックします。
        /// </summary>
        /// <param name="ch">チェック対象の文字</param>
        /// <returns>含まれる場合<c>true</c></returns>
        public bool Contains(char ch)
        {
            return Start <= ch && ch <= End;
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("CharRange(Start = '{0}', End = '{1}')", Start, End);
        }
    }
}
