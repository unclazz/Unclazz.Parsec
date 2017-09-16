using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    /// <summary>
    /// <see cref="ITextReader"/>の実装を容易にするための抽象クラスです。
    /// <para>
    /// 具象クラスは<see cref="Peek"/>と<see cref="ReadSimply"/>、
    /// およびこの抽象クラスが継承するより上位の抽象クラスが宣言する抽象メンバーを実装する必要があります。
    /// </para>
    /// </summary>
    abstract class AbstractTextReader : AutoDispose, ITextReader
    {
        public CharPosition Position { get; protected set; }
        public bool EndOfFile => Peek() == -1;

        public abstract int Peek();
        /// <summary>
        /// データソースから現在の文字位置の文字を読み取って返します。
        /// <para>
        /// データソースから読み取った文字を呼び出し元に返す前に、文字位置を次に進めます。
        /// このメソッドは読み取りに専念し<see cref="Position"/>の値の更新は行いません。
        /// </para>
        /// </summary>
        /// <returns>文字</returns>
        public abstract int ReadSimply();
        public int Read()
        {
            var curr = ReadSimply();
            if (curr == '\n' || (curr == '\r' && Peek() != '\n'))
            {
                Position = Position.NextLine;
            }
            else if (curr > -1)
            {
                Position = Position.NextColumn;
            }
            return curr;
        }
        public string ReadLine()
        {
            if (EndOfFile) return null;
            var startedOn = Position.Line;
            var buff = new StringBuilder();
            while (startedOn == Position.Line && !EndOfFile)
            {
                var ch = Read();
                if (ch != '\r' && ch != '\n') buff.Append((char)ch);
            }
            return buff.ToString();
        }
        public string ReadToEnd()
        {
            if (EndOfFile) return null;
            var buff = new StringBuilder();
            while (!EndOfFile)
            {
                buff.Append((char)Read());
            }
            return buff.ToString();
        }
    }
}
