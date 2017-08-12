using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClass
{
    /// <summary>
    /// 文字クラス（文字の概念的な集合）を表すクラスです。
    /// </summary>
    public abstract class CharClass
    {
        /// <summary>
        /// 文字の範囲から文字クラスを生成します。
        /// </summary>
        /// <param name="range">文字の範囲</param>
        public static implicit operator CharClass(CharRange range)
        {
            return new SingleCharRangeCharClass(range);
        }
        /// <summary>
        /// デリゲートから文字クラスを生成します。
        /// </summary>
        /// <param name="func">任意の文字がクラスに含まれるかどうかをチェックするデリゲート</param>
        public static implicit operator CharClass(Func<char, bool> func)
        {
            return For(func);
        }
        /// <summary>
        /// 2つの文字クラスを内容とする上位の文字クラスを生成します。
        /// <see cref="Union(CharClass)"/>と同じ動作をします。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass operator|(CharClass left, CharClass right)
        {
            return left.Union(right);
        }
        /// <summary>
        /// 文字クラスに文字を追加した文字クラスを生成します。
        /// <see cref="Plus(char)"/>と同じ動作をします。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass operator +(CharClass left, char right)
        {
            return left.Plus(right);
        }
        /// <summary>
        /// 文字クラスに文字の範囲を追加した文字クラスを生成します。
        /// <see cref="Plus(CharRange)"/>と同じ動作をします。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass operator +(CharClass left, CharRange right)
        {
            return left.Plus(right);
        }
        /// <summary>
        /// 文字クラスにUnicodeカテゴリを追加した文字クラスを生成します。
        /// <see cref="Plus(UnicodeCategory)"/>と同じ動作をします。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass operator +(CharClass left, UnicodeCategory right)
        {
            return left.Plus(right);
        }
        /// <summary>
        /// 文字クラスの補集合となる文字クラスを生成します。
        /// <see cref="Not(CharClass)"/>と同じ動作をします。
        /// </summary>
        /// <param name="operand">被演算子</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass operator !(CharClass operand)
        {
            return Not(operand);
        }

        public static CharClass For(Func<char, bool> func)
        {
            return new DelegateCharClass(func);
        }
        public static CharClass Not(CharClass clazz)
        {
            return new ComplementCharClass(clazz);
        }

        public abstract bool Contains(char ch);
        public virtual CharClass Union(CharClass other)
        {
            return new UnionCharClass(this, other);
        }
        public virtual CharClass Plus(char ch)
        {
            return new UnionCharClass(this, new SingleCharacterCharClass(ch));
        }
        public virtual CharClass Plus(CharRange range)
        {
            return new UnionCharClass(this, new SingleCharRangeCharClass(range));
        }
        public virtual CharClass Plus(UnicodeCategory cate)
        {
            return new UnionCharClass(this, new SingleUnicodeCategoryCharClass(cate));
        }
    }
}
