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
    public struct Capture<T>
    {
        public static implicit operator Capture<T>(T value)
        {
            return new Capture<T>(value);
        }
        public static implicit operator T(Capture<T> capture)
        {
            return capture.OrDefault();
        }
        public static Capture<T> operator |(Capture<T> left, Capture<T> right)
        {
            return left.Or(right);
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="value">保持する値</param>
        public Capture(T value)
        {
            _hasValue = true;
            _value = value;
        }

        readonly bool _hasValue;
        readonly T _value;

        /// <summary>
        /// 値を保持している場合<c>true</c>
        /// </summary>
        public bool HasValue => _hasValue;
        /// <summary>
        /// このインスタンスが保持している値を返します。
        /// </summary>
        public T Value => _hasValue ? _value : throw new InvalidOperationException();

        public Capture<T> Filter(Func<T,bool> func)
        {
            return _hasValue && func(_value) ? this : new Capture<T>();
        }
        public Capture<U> FlatMap<U>(Func<T, Capture<U>> func)
        {
            return _hasValue ? func(_value) : new Capture<U>();
        }
        /// <summary>
        /// このインスタンスが値を保持している場合アクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfHasValue(Action<T> act)
        {
            if (_hasValue) act(_value);
        }
        /// <summary>
        /// このインスタンスが値を保持している場合第1引数のアクションを実行し、
        /// そうでない場合第2引数のアクションを実行します。
        /// </summary>
        /// <param name="act">値を保持している場合に実行されるアクション</param>
        /// <param name="orElse">値を保持していない場合に実行されるアクション</param>
        public void IfHasValue(Action<T> act, Action orElse)
        {
            if (_hasValue) act(_value);
            else orElse();
        }
        /// <summary>
        /// このインスタンスが保持する値を引数で指定された関数で変換し新しいインスタンスでくるんで返します。
        /// インスタンスが値を保持していない場合、新しい空のインスタンスが返されます。
        /// </summary>
        /// <typeparam name="U">変換後の値の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <returns>変換後の値を保持するインスタンス</returns>
        public Capture<U> Map<U>(Func<T, U> func)
        {
            return _hasValue ? new Capture<U>(func(_value)) : new Capture<U>();
        }
        public Capture<T> Or(Capture<T> other)
        {
            return _hasValue ? this : other;
        }
        public T OrDefault()
        {
            return _hasValue ? _value : default(T);
        }
        /// <summary>
        /// 値を保持している場合はその値を、そうでない場合は引数で指定された値を返します。
        /// </summary>
        /// <param name="value">このインスタンスが値を保持していない場合にこのメソッドが返す値</param>
        /// <returns>値</returns>
        public T OrElse(T value)
        {
            return _hasValue ? _value : value;
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            if (_hasValue) return string.Format("Capture({0})", ParsecUtility.ValueToString(_value));
            else return "Capture()";
        }
    }
}
