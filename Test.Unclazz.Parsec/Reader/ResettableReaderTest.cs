using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.Reader;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class ResettableReaderTest
    {
        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 1)]
        [TestCase("0123456789X", 3, 0)]
        [TestCase("0123456789X", 0, 3)]
        [TestCase("0123456789X", 0, 1)]
        [TestCase("0123456789X", 0, 0)]
        [TestCase("0123456789X", 10, 0)]
        [TestCase("0123456789X", 11, 0)]
        [Description("Mark - Case #1 - 任意の位置でMarkを呼び出しリセットを準備できること")]
        public void Mark_Case1(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);

            // Act
            r.Mark();

            // Assert
            Repeats(r.Read, readCountAfterMark);
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo(willMarkOn < text.Length ? text[willMarkOn] : -1));
        }

        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 0)]
        [Description("Mark - Case #2 - Markは同じ文字位置で複数回呼び出しても問題ないこと")]
        public void Mark_Case2(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);

            // Act
            r.Mark();
            r.Mark();

            // Assert
            Repeats(r.Read, readCountAfterMark);
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo(text[willMarkOn]));
        }

        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 0)]
        [Description("Mark - Case #3 - Markは同じ文字位置で複数回呼び出しても個々の呼び出しごとにマークを付けること")]
        public void Mark_Case3(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);

            // Act
            r.Mark();
            r.Mark();

            // Assert
            Repeats(r.Read, readCountAfterMark);
            r.Unmark();
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo(text[willMarkOn]));
        }

        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 0)]
        [Description("Mark - Case #3 - Markを呼び出した回数だけUnmarkを呼び出すとマークはなくなること")]
        public void Mark_Case4(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);

            // Act
            r.Mark();
            r.Mark();

            // Assert
            Repeats(r.Read, readCountAfterMark);
            r.Unmark();
            r.Unmark();
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo(text[willMarkOn + readCountAfterMark]));
        }

        [Test]
        [Description("Unmark - Case #1 - Mark回数 ＜ Unmark回数 でもエラーとはならないこと")]
        public void Unmark_Case1()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));
            Repeats(r.Read, 3);
            r.Mark();

            // Act
            // Assert
            r.Unmark();
            r.Unmark();
        }

        [Test]
        [Description("Unmark - Case #2 - Unmarkは直近のMarkの結果を取り消すがそれ以前のMark結果はそのままとすること")]
        public void Unmark_Case2()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 3);
            Assert.That(r.Peek(), Is.EqualTo('3'));

            r.Mark();
            Repeats(r.Read, 3);
            Assert.That(r.Peek(), Is.EqualTo('6'));

            r.Mark();
            Repeats(r.Read, 3);
            Assert.That(r.Peek(), Is.EqualTo('9'));

            r.Unmark();
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo('3'));
            Assert.That(ReadsAll(r), Is.EqualTo("3456789X"));
        }

        [Test]
        [Description("Unmark - Case #3 - Unmarkは直近のMarkの結果を取り消すがそれ以前のMark結果はそのままとすること(2)")]
        public void Unmark_Case3()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('2'));

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('4'));

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('6'));

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('8'));

            r.Unmark();
            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo('4'));
            Assert.That(ReadsAll(r), Is.EqualTo("456789X"));
        }

        [Test]
        [Description("Unmark - Case #4 - UnmarkですべてのMarkを取り消したあと再度Mark/Resetを行っても正しく動作すること")]
        public void Unmark_Case4()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('2'));

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('4'));

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('6'));

            r.Unmark();
            r.Unmark();

            r.Mark();
            Repeats(r.Read, 2);
            Assert.That(r.Peek(), Is.EqualTo('8'));

            r.Reset();
            Assert.That(r.Peek(), Is.EqualTo('6'));
            Assert.That(ReadsAll(r), Is.EqualTo("6789X"));
        }

        [TestCase("abcdef", 0, 1, 1)]
        [TestCase("abcdef", 0, 3, 1)]
        [TestCase("abcdef", 1, 3, 1)]
        [TestCase("abcdef", 1, 4, 1)]
        [TestCase("abcdef", 0, 1, 2)]
        [TestCase("abcdef", 0, 4, 2)]
        [Description("Reset - Case #1 - ResetによりPeek/Positionの値が正しく更新されることをチェック")]
        public void Reset_Case1(string text, int willMarkOn, int willResetOn, int willBreakOn)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (willMarkOn == r.Position.Index)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                if (willResetOn == r.Position.Index)
                {
                    // Act
                    r.Reset();

                    // Assert
                    Assert.That(r.Position.Index, Is.EqualTo(willMarkOn));
                    Assert.That(r.Peek(), Is.EqualTo(markedChar));

                    if (--willBreakOn < 1) break;
                }
                r.Read();
            }
        }
        [TestCase("abcdef", 0)]
        [TestCase("abcdef", 1)]
        [Description("Reset - Case #2 - ResetによりPeek/Positionの値が正しく更新されることをチェック")]
        public void Reset_Case2(string text, int willMarkOn)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (willMarkOn == r.Position.Index)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                r.Read();
            }

            // Act
            r.Reset();

            // Assert
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn));
            Assert.That(r.Peek(), Is.EqualTo(markedChar));
        }

        [Test]
        [Description("Reset - Case #3 - Read/Resetを繰り返しても正しい読み取りが約束されることをチェック")]
        public void Reset_Case3()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('2'));

            r.Mark(); // 01[2]3456789X

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('2'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Reset();
            Assert.That((char) r.Peek(), Is.EqualTo('2'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('6'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('2'));
        }
        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 1)]
        [TestCase("0123456789X", 3, 0)]
        public void Capture_Case1(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);
            r.Mark();
            Repeats(r.Read, readCountAfterMark);

            // Act
            var cap = r.Capture(true);

            // Assert
            Assert.That(cap, Is.EqualTo(text.Substring(willMarkOn, readCountAfterMark)));
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn + readCountAfterMark));
            r.Reset();
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn + readCountAfterMark));
        }
        [TestCase("0123456789X", 3, 3)]
        [TestCase("0123456789X", 3, 1)]
        [TestCase("0123456789X", 3, 0)]
        public void Capture_Case2(string text, int willMarkOn, int readCountAfterMark)
        {
            // Arrange
            var r = new ResettableReader(new StringReader(text));
            Repeats(r.Read, willMarkOn);
            r.Mark();
            Repeats(r.Read, readCountAfterMark);

            // Act
            var cap = r.Capture(false);

            // Assert
            Assert.That(cap, Is.EqualTo(text.Substring(willMarkOn, readCountAfterMark)));
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn + readCountAfterMark));
            r.Reset();
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn));
        }

        void Repeats(Action act, int times)
        {
            foreach (var i in Enumerable.Range(1, times))
            {
                act();
            }
        }
        void Repeats<T>(Func<T> func, int times)
        {
            Repeats(() => { func(); }, times);
        }
        string ReadsAll(ITextReader r)
        {
            var buff = new StringBuilder();
            while (!r.EndOfFile)
            {
                buff.Append((char)r.Read());
            }
            return buff.ToString();
        }
    }
}
