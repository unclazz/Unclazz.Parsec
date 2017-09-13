using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 文字クラス（文字の概念的な集合）を表すクラスです。
    /// </summary>
    public abstract class CharClass
    {
        static CharClass _alphabetic;
        static CharClass _numeric;
        static CharClass _alphanumeric;
        static CharClass _hexDigit;
        static CharClass _control;
        static CharClass _spaceAndControl;
        static CharClass _newline;

        /// <summary>
        /// CRLFを表す文字クラスです。
        /// </summary>
        public static CharClass Newline => _newline ?? (_newline = AnyOf('\r', '\n'));
        /// <summary>
        /// <c>'a'</c>から<c>'z'</c>と<c>'A'</c><c>'Z'</c>の範囲の文字を含むクラスです。
        /// </summary>
        public static CharClass Alphabetic => _alphabetic ?? (_alphabetic = Between('a', 'z') | Between('A', 'Z'));
        /// <summary>
        /// <c>'0'</c>から<c>'9'</c>の範囲の文字を含むクラスです。
        /// </summary>
        public static CharClass Numeric => _numeric ?? (_numeric = Between('0', '9'));
        /// <summary>
        /// <see cref="Alphabetic"/>と<see cref="Numeric"/>を合わせたクラスです。
        /// </summary>
        public static CharClass Alphanumeric => _alphanumeric ?? (_alphanumeric = Alphabetic | Numeric);
        /// <summary>
        /// 16進数の数字を表す文字クラスです。
        /// </summary>
        public static CharClass HexDigit => _hexDigit ?? (_hexDigit = Between('0', '9') | Between('a', 'f') | Between('A', 'F'));
        /// <summary>
        /// 制御文字（コードポイントで<c>0</c>から<c>31</c>と<c>127</c>）を内容とするクラスです。
        /// </summary>
        public static CharClass Control => _control ?? (_control = Between((char)0, (char)31) + (char)127);
        /// <summary>
        /// <see cref="Control"/>と空白文字（コードポイントで<c>32</c>）を内容とするクラスです。
        /// </summary>
        public static CharClass SpaceAndControl => _spaceAndControl ?? (_spaceAndControl = Between((char)0, (char)32) + (char)127);

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
        /// <summary>
        /// 述語関数を利用して文字クラスを生成します。
        /// </summary>
        /// <param name="pred">ある文字がクラスに属するかを判定する述語関数</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass For(Func<char, bool> pred)
        {
            return new PredicateCharClass(pred);
        }
        /// <summary>
        /// 述語関数を利用して文字クラスを生成します。
        /// </summary>
        /// <param name="pred">ある文字がクラスに属するかを判定する述語関数</param>
        /// <param name="desc">文字クラスの説明</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass For(Func<char, bool> pred, string desc)
        {
            return new PredicateCharClass(pred);
        }
        /// <summary>
        /// ある文字クラスに属さない文字のクラス（集合の補集合）を生成します。
        /// </summary>
        /// <param name="clazz">元になる文字クラス</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass Not(CharClass clazz)
        {
            return new ComplementCharClass(clazz);
        }
        /// <summary>
        /// 文字集合から文字クラスを生成します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass AnyOf(params char[] chars)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (chars.Length == 0) throw new ArgumentException("character group is empty.");
            return new CharRangeCharClass(CharRangeUtility.AnyOf(chars));
        }
        /// <summary>
        /// 文字集合から文字クラスを生成します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass AnyOf(IEnumerable<char> chars)
        {
            return AnyOf(chars.ToArray());
        }
        /// <summary>
        /// Unicodeカテゴリーから文字クラスを生成します。
        /// </summary>
        /// <param name="categories">Unicodeカテゴリー</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass AnyOf(params UnicodeCategory[] categories)
        {
            if (categories == null) throw new ArgumentNullException(nameof(categories));
            if (categories.Length == 0) throw new ArgumentException("category group is empty.");
            return new UnicodeCategoryCharClass(categories);
        }
        /// <summary>
        /// 文字の範囲から文字クラスを生成します。
        /// </summary>
        /// <param name="start">範囲の始点</param>
        /// <param name="end">範囲の終点</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass Between(char start, char end)
        {
            return new CharRangeCharClass(CharRange.Between(start, end));
        }
        /// <summary>
        /// 文字から文字クラス（1文字のみがそのメンバーとなる文字クラス）を生成します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しい文字クラス</returns>
        public static CharClass Exact(char ch)
        {
            return new CharRangeCharClass(CharRange.Exactly(ch));
        }

        /// <summary>
        /// この文字クラスが引数で指定された文字を含むかどうかをチェックします。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>当該の文字を含む場合<c>true</c></returns>
        public abstract bool Contains(char ch);
        /// <summary>
        /// この文字クラスの内容を表現する説明です。
        /// <see cref="ToString"/>メソッドで使用されます。
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// この文字クラスと別の文字クラスを統合した新しい文字クラスを生成します。
        /// </summary>
        /// <param name="another">別の文字クラス</param>
        /// <returns>新しい文字クラス</returns>
        public virtual CharClass Union(CharClass another)
        {
            return new UnionCharClass(this, another);
        }
        /// <summary>
        /// この文字クラスに引数で指定された文字を加えます。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しい文字クラス</returns>
        public virtual CharClass Plus(char ch)
        {
            return new UnionCharClass(this, Exact(ch));
        }
        /// <summary>
        /// この文字クラスに引数で指定されたUnicodeカテゴリーを加えます。
        /// </summary>
        /// <param name="cate">Unicodeカテゴリー</param>
        /// <returns>新しい文字クラス</returns>
        public virtual CharClass Plus(UnicodeCategory cate)
        {
            return new UnionCharClass(this, new UnicodeCategoryCharClass(cate));
        }
        /// <summary>
        /// この文字クラスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("class ({0})", Description);
        }
    }
}
