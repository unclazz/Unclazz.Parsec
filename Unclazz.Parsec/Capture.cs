using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーによりキャプチャされた値を保持する列挙型です。
    /// </summary>
    /// <typeparam name="T">保持する値の型</typeparam>
    public struct Capture<T> : IEnumerable<T>
    {
        static readonly T[] _empty = new T[0];

        /// <summary>
        /// 新しい要素を追加します。
        /// </summary>
        /// <param name="left">元のインスタンス</param>
        /// <param name="right">新しい要素</param>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> operator +(Capture<T> left, T right)
        {
            return left.Add(right);
        }
        /// <summary>
        /// 2つのインスタンスの要素を統合します。
        /// </summary>
        /// <param name="left">1つめのインスタンス</param>
        /// <param name="right">2つめのインスタンス</param>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> operator +(Capture<T> left, Capture<T> right)
        {
            return left.Union(right);
        }

        /// <summary>
        /// 要素0のインスタンスを返します。
        /// </summary>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> OfEmpty()
        {
            return new Capture<T>();
        }
        /// <summary>
        /// 要素1のインスタンスを返します。
        /// </summary>
        /// <param name="item">要素</param>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> OfSingle(T item)
        {
            return new Capture<T>(item);
        }
        /// <summary>
        /// 指定された配列内容を要素とするインスタンスを返します。
        /// </summary>
        /// <param name="items">要素配列</param>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> Of(params T[] items)
        {
            return new Capture<T>(items);
        }
        /// <summary>
        /// 指定されたシーケンスの内容を要素とするインスタンスを返します。
        /// </summary>
        /// <param name="items">要素シーケンス</param>
        /// <returns>新しいインスタンス</returns>
        public static Capture<T> Of(IEnumerable<T> items)
        {
            return Of(items.ToArray());
        }

        Capture(T item)
        {
            _hasValue = true;
            _items = new[] { item };
        }
        Capture(T[] items)
        {
            _hasValue = (items != null && items.Length > 0);
            _items = items;
        }

        readonly bool _hasValue;
        readonly T[] _items;

        /// <summary>
        /// このインスタンスが保持している値のうち添字で指定されたものを返します。
        /// </summary>
        /// <param name="i">添字</param>
        /// <returns>要素</returns>
        public T this[int i] => _hasValue ? _items[i] : throw new IndexOutOfRangeException();

        /// <summary>
        /// 値を保持している場合<c>true</c>
        /// </summary>
        public bool HasValue => _hasValue;
        /// <summary>
        /// このインスタンスが保持している値の要素数です。
        /// </summary>
        public int Count => _hasValue ? _items.Length : 0;
        /// <summary>
        /// 指定された要素を追加した新しいインスタンスを返します。
        /// </summary>
        /// <param name="item">新しい要素</param>
        /// <returns>新しいインスタンス</returns>
        public Capture<T> Add(T item)
        {
            if (_hasValue)
            {
                T[] newItems = new T[_items.Length + 1];
                _items.CopyTo(newItems, 0);
                newItems[_items.Length] = item;
                return new Capture<T>(newItems);
            }
            else
            {
                return new Capture<T>(item);
            }
        }
        /// <summary>
        /// 2つのインスタンスの要素を統合した新しいインスタンスを返します。
        /// </summary>
        /// <param name="other">別のインスタンス</param>
        /// <returns>新しいインスタンス</returns>
        public Capture<T> Union(Capture<T> other)
        {
            if (_hasValue)
            {
                if (other._hasValue)
                {
                    T[] newItems = new T[_items.Length + other._items.Length];
                    _items.CopyTo(newItems, 0);
                    other._items.CopyTo(newItems, _items.Length);
                    return new Capture<T>(newItems);
                }
                return this;
            }
            return other;
        }
        /// <summary>
        /// このインスタンスが保持する値を引数で指定された関数で変換し新しいインスタンスでくるんで返します。
        /// インスタンスが値を保持していない場合、新しい空のインスタンスが返されます。
        /// </summary>
        /// <typeparam name="U">変換後の値の型</typeparam>
        /// <param name="transform">変換を行う関数</param>
        /// <returns>変換後の値を保持するインスタンス</returns>
        public Capture<U> Map<U>(Func<T, U> transform)
        {
            if (_hasValue)
            {
                var u = new U[_items.Length];
                for (var i = 0; i < _items.Length; i++) u[i] = transform(_items[i]);
                return new Capture<U>(u);
            }
            return new Capture<U>();
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            if (_hasValue) return string.Format("Capture({0})", ParsecUtility.ValueToString(_items));
            else return "Capture()";
        }
        /// <summary>
        /// このインスタンスが持つ要素を返す<see cref="IEnumerator{T}"/>を取得します。
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/>インスタンス</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)(_hasValue ? _items : _empty)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 最初の要素を取得します。
        /// 要素0のインスタンスの場合、引数で指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>最初の要素</returns>
        public T FirstOrElse(T defaultValue)
        {
            return _hasValue ? _items[0] : defaultValue;
        }
        /// <summary>
        /// 最後の要素を取得します。
        /// 要素0のインスタンスの場合、引数で指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>最後の要素</returns>
        public T LastOrElse(T defaultValue)
        {
            return _hasValue ? _items[_items.Length - 1] : defaultValue;
        }
        /// <summary>
        /// 指定された位置の要素を取得します。
        /// 指定された位置の要素が存在しない場合、引数で指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="i">要素の位置</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>指定された位置の要素</returns>
        public T GetOrElse(int i, T defaultValue)
        {
            return _hasValue && i < _items.Length ? _items[i] : defaultValue; 
        }
        /// <summary>
        /// 要素ごとに指定されたアクションを呼び出します。
        /// 要素0のインスタンスの場合は何も行われません。
        /// </summary>
        /// <param name="act">アクション（第1引数には要素が指定される）</param>
        public void ForEach(Action<T> act)
        {
            if (!_hasValue) return;
            for (var i = 0; i < _items.Length; i++) act(_items[i]);
        }
        /// <summary>
        /// 要素ごとに指定されたアクションを呼び出します。
        /// 要素0のインスタンスの場合は何も行われません。
        /// </summary>
        /// <param name="act">アクション（第1引数には要素が、第2引数には添字が指定される）</param>
        public void ForEach(Action<T, int> act)
        {
            if (!_hasValue) return;
            for (var i = 0; i < _items.Length; i++) act(_items[i], i);
        }
    }
}
