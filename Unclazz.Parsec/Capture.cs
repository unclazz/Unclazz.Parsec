using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーによりキャプチャされた値を保持する（もしくは保持しない）列挙型です。
    /// </summary>
    /// <typeparam name="T">保持する値の型</typeparam>
    public struct Capture<T>
    {
        /// <summary>
        /// 値を保持するインスタンスを生成するコンストラクタです。
        /// </summary>
        /// <param name="value">値</param>
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
        /// 値を取得します。
        /// このインスタンスが値を保持していない場合は例外をスローします。
        /// </summary>
        public T Value
        {
            get
            {
                if (_hasValue) return _value;
                else throw new InvalidOperationException();
            }
        }
        /// <summary>
        /// 値を保持している場合はその値を、そうでない場合は引数で指定された値を返します。
        /// </summary>
        /// <param name="value">このインスタンスが値を保持していない場合にこのメソッドが返す値</param>
        /// <returns>値</returns>
        public T OrElse(T value)
        {
            if (_hasValue) return _value;
            else return value;
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
            if (_hasValue) return new Capture<U>(transform(_value));
            else return new Capture<U>();
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
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            if (_hasValue)
            {
                return string.Format("Capture({0}, type = {1})",
                    ParsecUtility.ValueToString(_value),
                    ParsecUtility.TypeToString(typeof(T)));
            }
            else
            {
                return string.Format("Capture(type = {0})",
                    ParsecUtility.TypeToString(typeof(T)));
            }
        }
    }
}
