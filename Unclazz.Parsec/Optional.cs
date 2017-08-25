using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 値が存在しない可能性のあることを示すコンテナです。
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    public struct Optional<T>
    {
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="value">コンテナに格納される値</param>
        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="capture">コンテナ</param>
        public static implicit operator T(Optional<T> capture)
        {
            return capture.OrDefault();
        }
        /// <summary>
        /// 左辺のコンテナの値が存在する場合は左辺を、それ以外の場合は右辺のコンテナを返します。
        /// </summary>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>コンテナ</returns>
        public static Optional<T> operator |(Optional<T> left, Optional<T> right)
        {
            return left.Or(right);
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="value">保持する値</param>
        public Optional(T value)
        {
            _hasValue = true;
            _value = value;
        }

        readonly bool _hasValue;
        readonly T _value;

        /// <summary>
        /// 値が存在する場合<c>true</c>
        /// </summary>
        public bool Present => _hasValue;
        /// <summary>
        /// コンテナから値を取り出します。
        /// </summary>
        /// <exception cref="InvalidOperationException">値が存在しない場合</exception>
        public T Value => _hasValue ? _value : throw new InvalidOperationException();
        /// <summary>
        /// 値が存在しかつ述語関数が<c>true</c>を返す場合コンテナ自身を返し、それ以外の場合は空のコンテナを返します。
        /// </summary>
        /// <param name="func">関数</param>
        /// <returns>コンテナ</returns>
        public Optional<T> Filter(Func<T,bool> func)
        {
            return _hasValue && func(_value) ? this : new Optional<T>();
        }
        /// <summary>
        /// 値が存在する場合は関数の戻り値を返し、それ以外の場合は空のコンテナを返します。
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="func">関数</param>
        /// <returns>コンテナ</returns>
        public Optional<U> FlatMap<U>(Func<T, Optional<U>> func)
        {
            return _hasValue ? func(_value) : new Optional<U>();
        }
        /// <summary>
        /// 値を存在する場合アクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfPresent(Action<T> act)
        {
            if (_hasValue) act(_value);
        }
        /// <summary>
        /// 値が存在する場合は第1引数のアクションを実行し、
        /// そうでない場合は第2引数のアクションを実行します。
        /// </summary>
        /// <param name="act">値を保持している場合に実行されるアクション</param>
        /// <param name="orElse">値を保持していない場合に実行されるアクション</param>
        public void IfPresent(Action<T> act, Action orElse)
        {
            if (_hasValue) act(_value);
            else orElse();
        }
        /// <summary>
        /// 値が存在する場合関数の戻り値をコンテナにくるんで返します。
        /// それ以外の場合は空のコンテナが返されます。
        /// </summary>
        /// <typeparam name="U">変換後の値の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <returns>変換後の値を保持するインスタンス</returns>
        public Optional<U> Map<U>(Func<T, U> func)
        {
            return _hasValue ? new Optional<U>(func(_value)) : new Optional<U>();
        }
        /// <summary>
        /// レシーバーのコンテナの値が存在する場合はレシーバーを、それ以外の場合は引数のコンテナを返します。
        /// </summary>
        /// <param name="other">他のコンテナ</param>
        /// <returns>コンテナ</returns>
        public Optional<T> Or(Optional<T> other)
        {
            return _hasValue ? this : other;
        }
        /// <summary>
        /// 値が存在する場合はその値を、それ以外の場合は型パラメータで指定された型のデフォルト値を返します。
        /// </summary>
        /// <returns>値</returns>
        public T OrDefault()
        {
            return _hasValue ? _value : default(T);
        }
        /// <summary>
        /// 値が存在する場合はその値を、そうでない場合は引数で指定された値を返します。
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
            if (_hasValue) return string.Format("Optional({0})", ParsecUtility.ValueToString(_value));
            else return "Optional()";
        }
    }
}
