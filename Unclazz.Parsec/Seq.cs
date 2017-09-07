using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// イミュータブルなシーケンスです。
    /// </summary>
    /// <typeparam name="T">要素の型</typeparam>
    public class Seq<T> : IEnumerable<T>
    {
        /// <summary>
        /// 2つのシーケンスを連結した新しいシーケンスを返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Seq<T> operator +(Seq<T> left, Seq<T> right)
        {
            return left.Concat(right);
        }
        /// <summary>
        /// 2つのシーケンスを連結した新しいシーケンスを返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Seq<T> operator +(Seq<T> left, IEnumerable<T> right)
        {
            return left.ConcatRight(right);
        }
        /// <summary>
        /// 2つのシーケンスを連結した新しいシーケンスを返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Seq<T> operator +(IEnumerable<T> left, Seq<T> right)
        {
            return right.ConcatLeft(left);
        }
        /// <summary>
        /// 先頭に新しい要素を追加したシーケンスを返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Seq<T> operator +(T left, Seq<T> right)
        {
            return right.Prepend(left);
        }
        /// <summary>
        /// 末尾に新しい要素を追加したシーケンスを返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Seq<T> operator +(Seq<T> left, T right)
        {
            return left.Append(right);
        }

        /// <summary>
        /// 空のシーケンスです。
        /// </summary>
        public static Seq<T> Empty { get; } = new Seq<T>(new T[0]);
        /// <summary>
        /// 指定した要素を持つシーケンスを返します。
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Seq<T> Of(IEnumerable<T> items)
        {
            return Of(items?.ToArray());
        }
        /// <summary>
        /// 指定した要素を持つシーケンスを返します。
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Seq<T> Of(params T[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            // 配列が空ならレディメイドの空のシーケンスを返す
            if (items.Length == 0) return Empty;

            // 配列は念のためコピーをとることで不変性を確立する
            var privateCopy = new T[items.Length];
            items.CopyTo(privateCopy, 0);

            // インスタンス化を行う
            return  new Seq<T>(privateCopy);
        }

        Seq(T[] items) {
            _items = items;
            Count = items.Length;
        }

        readonly T[] _items;

        /// <summary>
        /// シーケンスの要素数です。
        /// </summary>
        public int Count { get; }
        /// <summary>
        /// シーケンスの要素に添え字アクセスします。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i] => _items[i];

        /// <summary>
        /// シーケンスの列挙子を取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }
        /// <summary>
        /// シーケンスの列挙子を取得します。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// シーケンスの文字列表現です。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Count == 0) return "[]";
            return _items.Aggregate(new StringBuilder(),
                (a, b) => a.Append(a.Length == 0 ? "[" : ", ").Append(b),
                b => b.Append(']').ToString());
        }
        /// <summary>
        /// 先頭に新しい要素を追加したシーケンスを返します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Seq<T> Append(T item)
        {
            if (Count == 0) new Seq<T>(new[] { item });

            var newItems = new T[_items.Length + 1];
            Array.Copy(_items, newItems, _items.Length);
            newItems[_items.Length] = item;
            return new Seq<T>(newItems);
        }
        /// <summary>
        /// 末尾に新しい要素を追加したシーケンスを返します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Seq<T> Prepend(T item)
        {
            if (Count == 0) new Seq<T>(new[] { item });

            var newItems = new T[_items.Length + 1];
            Array.Copy(_items, 0, newItems, 1, _items.Length);
            newItems[0] = item;
            return new Seq<T>(newItems);
        }
        /// <summary>
        /// 2つのシーケンスを連結した新しいシーケンスを返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Seq<T> Concat(Seq<T> other)
        {
            if (Count == 0) return other;
            if (other.Count == 0) return this;

            return ConcatRight(other._items);
        }
        /// <summary>
        /// シーケンスの要素ごとにアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        public void ForEach(Action<T, int> act)
        {
            if (Count == 0) return;
            for (var i = 0; i < _items.Length; i++) act(_items[i], i);
        }
        /// <summary>
        /// シーケンスの要素ごとにアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        public void ForEach(Action<T> act)
        {
            ForEach((a, b) => act(a));
        }
        /// <summary>
        /// シーケンスの要素に関数を適用した新しいシーケンスを返します。
        /// </summary>
        /// <typeparam name="U">新しいシーケンスの要素の型</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Seq<U> Map<U>(Func<T, U> func)
        {
            return Map((a, b) => func(a));
        }
        /// <summary>
        /// シーケンスの要素に関数を適用した新しいシーケンスを返します。
        /// </summary>
        /// <typeparam name="U">新しいシーケンスの要素の型</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Seq<U> Map<U>(Func<T, int, U> func)
        {
            if (Count == 0) return Seq<U>.Empty;

            var newItems = new U[_items.Length];
            for (var i = 0; i < _items.Length; i++)
            {
                newItems[i] = func(_items[i], i);
            }
            return new Seq<U>(newItems);
        }
        /// <summary>
        /// このシーケンスの右側（末尾側）に引数で指定したコレクションを連結する。
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        Seq<T> ConcatRight(IEnumerable<T> right)
        {
            if (Count == 0) return new Seq<T>(right.ToArray());
            return new Seq<T>(ConcatArrays(_items, right.ToArray()));
        }
        /// <summary>
        /// このシーケンスの左側（先頭側）に引数で指定したコレクションを連結する。
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        Seq<T> ConcatLeft(IEnumerable<T> left)
        {
            var leftArray = left;

            if (Count == 0) return new Seq<T>(left.ToArray());
            return new Seq<T>(ConcatArrays(left.ToArray(), _items));
        }
        T[] ConcatArrays(T[] left, T[] right)
        {
            var leftLen = left.Length; // 左側シーケンス長
            var rightLen = right.Length; // 右側シーケンス長
            var newArray = new T[leftLen + rightLen]; // 新しい配列を初期化

            // 左側シーケンスを新しい配列にコピー（始点＝0）
            // 右側シーケンスを新しい配列にコピー（始点＝左側シーケンス末尾の次）
            left.CopyTo(newArray, 0);
            right.CopyTo(newArray, left.Length);

            return newArray;
        }
    }
}
