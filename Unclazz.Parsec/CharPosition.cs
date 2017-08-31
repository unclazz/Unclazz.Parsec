using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 文字位置を表す構造体です。
    /// </summary>
    public struct CharPosition
    {
        static readonly CharPosition _start = new CharPosition(0, 1, 1);
        /// <summary>
        /// データソースの先頭を表すインスタンスです。
        /// </summary>
        public static CharPosition BeginningOfFile => _start;
        /// <summary>
        /// 2つのインスタンスの等値性を確認します。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>等値とみなせる場合<c>true</c></returns>
        public static bool operator ==(CharPosition left, CharPosition right)
        {
            return left.Equals(right);
        }
        /// <summary>
        /// 2つのインスタンスの等値性を確認します。
        /// </summary>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns>等値でない場合<c>true</c></returns>
        public static bool operator !=(CharPosition left, CharPosition right)
        {
            return !left.Equals(right);
        }

        int _index;
        int _linePosition;
        int _columnPosition;
        int _hashCodeCache;

        /// <summary>
        /// データソースの先頭を始点とする添字です。<c>0</c>始まりです。
        /// </summary>
        public int Index => _index;
        /// <summary>
        /// 行数です。<c>1</c>始まりです。
        /// </summary>
        public int Line => _linePosition < 1 ? 1 : _linePosition;
        /// <summary>
        /// 列数（現在の行の先頭を始点とする添字）です。<c>1</c>始まりです。
        /// </summary>
        public int Column => _columnPosition < 1 ? 1 : _columnPosition;

        CharPosition(int index, int linePosition, int columnPosition)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            _index = index;
            if (linePosition < 1) throw new ArgumentOutOfRangeException(nameof(linePosition));
            _linePosition = linePosition;
            if (columnPosition < 1) throw new ArgumentOutOfRangeException(nameof(columnPosition));
            _columnPosition = columnPosition;
            _hashCodeCache = 0;
        }

        /// <summary>
        /// 列数に<c>+1</c>された次の文字位置です。
        /// </summary>
        public CharPosition NextColumn => new CharPosition(Index + 1, Line, Column + 1);
        /// <summary>
        /// 行数に<c>+1</c>された次の文字位置です。
        /// </summary>
        public CharPosition NextLine => new CharPosition(Index + 1, Line + 1, 1);

        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("CharacterPosition(Line = {0}, Column = {1}, Index = {2})", Line, Column, Index);
        }
        /// <summary>
        /// ハッシュコードを返します。
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            if (_hashCodeCache == 0)
            {
                _hashCodeCache = 1 + Index.GetHashCode() + 
                    Line.GetHashCode() + Column.GetHashCode();
            }
            return _hashCodeCache;
        }
        /// <summary>
        /// 2つのインスタンスの等値性を確認します。
        /// </summary>
        /// <param name="obj">他のインスタンス</param>
        /// <returns>等値とみなせる場合<c>true</c></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is CharPosition)) return false;
            var that = (CharPosition)obj;
            return Line == that.Line && Column == that.Column && Index == that.Index;
        }
    }
}
