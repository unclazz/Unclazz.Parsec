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

        public static Capture<T> operator +(Capture<T> left, T right)
        {
            return left.Add(right);
        }
        public static Capture<T> operator +(Capture<T> left, Capture<T> right)
        {
            return left.Union(right);
        }

        public static Capture<T> OfEmpty()
        {
            return new Capture<T>();
        }
        public static Capture<T> OfSingle(T value)
        {
            return new Capture<T>(value);
        }
        public static Capture<T> OfMultiple(params T[] items)
        {
            return new Capture<T>(items);
        }
        public static Capture<T> OfMultiple(IEnumerable<T> items)
        {
            return OfMultiple(items.ToArray());
        }

        Capture(T value)
        {
            _hasValue = true;
            _value = new[] { value };
        }
        Capture(T[] values)
        {
            _hasValue = true;
            _value = values;
        }

        readonly bool _hasValue;
        readonly T[] _value;

        /// <summary>
        /// 値を保持している場合<c>true</c>
        /// </summary>
        public bool HasValue => _hasValue;
        /// <summary>
        /// 値を取得します。
        /// このインスタンスが値を保持していない場合は例外をスローします。
        /// </summary>
        public IEnumerable<T> Value => _hasValue ? _value : _empty;

        public Capture<T> Add(T item)
        {
            if (_hasValue)
            {
                T[] newItems = new T[_value.Length + 1];
                _value.CopyTo(newItems, 0);
                return new Capture<T>(newItems);
            }
            else
            {
                return new Capture<T>();
            }
        }
        public Capture<T> Union(Capture<T> other)
        {
            if (_hasValue)
            {
                if (other._hasValue)
                {
                    T[] newItems = new T[_value.Length + other._value.Length];
                    _value.CopyTo(newItems, 0);
                    other._value.CopyTo(newItems, _value.Length);
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
            if (_hasValue) return new Capture<U>(new U[0]);
            else return new Capture<U>();
        }
        public void ForEach(Action<T> act)
        {
            if (_hasValue) return;
            for (var i = 0; i < _value.Length; i++) act(_value[i]);
        }
        public void ForEach(Action<T, int> act)
        {
            if (_hasValue) return;
            for (var i = 0; i < _value.Length; i++) act(_value[i], i);
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("Capture({0})", ParsecUtility.ValueToString(_value));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_empty).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
