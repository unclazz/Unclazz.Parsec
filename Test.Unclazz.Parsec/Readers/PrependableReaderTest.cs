using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.Readers;
using static Test.Unclazz.Parsec.TestUtility;

namespace Test.Unclazz.Parsec.Readers
{
    [TestFixture]
    public class PrependableReaderTest
    {
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        [Description("Read - Case1 - コンストラクタに渡されたプレフィクスと本文を順番に返す")]
        public void Read_Case1(string prefixText, string readerText, int firstChar, int secondChar)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));

            // Act
            var ch0 = r.Read();
            var ch1 = r.Read();

            // Assert
            Assert.That(ch0, Is.EqualTo(firstChar));
            Assert.That(ch1, Is.EqualTo(secondChar));
        }
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        [Description("Peek - Case1 - 現在の文字位置の文字を返す")]
        public void Peek_Case1(string prefixText, string readerText, int firstChar, int secondChar)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));

            // Act
            var ch0 = r.Peek();
            r.Read();
            var ch1 = r.Peek();

            // Assert
            Assert.That(ch0, Is.EqualTo(firstChar));
            Assert.That(ch1, Is.EqualTo(secondChar));
        }
        [TestCase("", "abc", 1)]
        [TestCase("a", "bcd", 1)]
        [TestCase("def", "", 1)]
        [TestCase("de", "f", 1)]
        [TestCase("", "", 0)]
        [Description("Position.Index - Case1 - 現在の文字位置情報を返す")]
        public void PositionIndex_Case1(string prefixText, string readerText, int resultIndex)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));

            // Act
            var p0 = r.Position.Index;
            var ch = r.Read();
            var p1 = r.Position.Index;

            // Assert
            Assert.That(p0, Is.EqualTo(0));
            Assert.That(p1, Is.EqualTo(resultIndex));
        }
        [TestCase("", "abc", 3)]
        [TestCase("a", "bcd", 4)]
        [TestCase("def", "", 3)]
        [TestCase("de", "f", 3)]
        [TestCase("", "", 0)]
        [Description("Position.Index - Case2 - 現在の文字位置情報を返す（終端状態）")]
        public void PositionIndex_Case2(string prefixText, string readerText, int resultIndex)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            ConsumesAll(r);

            // Act
            var p1 = r.Position.Index;

            // Assert
            Assert.That(p1, Is.EqualTo(resultIndex));
        }
        [TestCase("", "ab\rcd")]
        [TestCase("a", "b\ncd")]
        [TestCase("de\rfg", "")]
        [TestCase("de\n", "fg")]
        [TestCase("de", "\nfg")]
        [TestCase("de", "\n")]
        [Description("Position.Line - Case1 - 現在の文字位置情報を返す")]
        public void PositionLine_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var l0 = r.Position.Line;
            r.Read();
            var l1 = r.Position.Line;

            // Assert
            Assert.That(l0, Is.EqualTo(1));
            Assert.That(l1, Is.EqualTo(2));
        }
        [TestCase("", "ab\r\ncd")]
        [TestCase("a", "b\r\ncd")]
        [TestCase("de\r\nfg", "")]
        [TestCase("de\r", "\nfg")]
        [Description("Position.Line - Case2 - 現在の文字位置情報を返す（終端状態）")]
        public void PositionLine_Case2(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var l0 = r.Position.Line;
            r.Read();
            var l1 = r.Position.Line;
            r.Read();
            var l2 = r.Position.Line;

            // Assert
            Assert.That(l0, Is.EqualTo(1));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(2));
        }
        [TestCase("", "ab\rcd")]
        [TestCase("a", "b\ncd")]
        [TestCase("de\rfg", "")]
        [TestCase("de\n", "fg")]
        [TestCase("de", "\nfg")]
        [TestCase("de", "\n")]
        [Description("Position.Column - Case1 - 現在の文字位置情報を返す")]
        public void PositionColumn_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var c0 = r.Position.Column;
            r.Read();
            var c1 = r.Position.Column;

            // Assert
            Assert.That(c0, Is.EqualTo(3));
            Assert.That(c1, Is.EqualTo(1));
        }
        [TestCase("", "abc")]
        [TestCase("a", "bc")]
        [TestCase("ab", "c")]
        [TestCase("abc", "")]
        [Description("Reattach - Case1 - 文字位置を引数で指定した値で初期化する")]
        public void Reattach_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            r.Read();

            // Act
            r.Reattach(CharPosition.BeginningOfFile, prefixText.ToArray());
            var p = r.Position.Index;
            var l = r.Position.Line;
            var c = r.Position.Column;
            var ch = r.Peek();

            // Assert
            Assert.That(p, Is.EqualTo(0));
            Assert.That(l, Is.EqualTo(1));
            Assert.That(c, Is.EqualTo(1));
            Assert.That(ch, Is.EqualTo(prefixText.Length == 0 ? readerText[1] : (prefixText + readerText)[0]));
        }
        [TestCase("", "0123456789")]
        [TestCase("0", "123456789")]
        [TestCase("01", "23456789")]
        [TestCase("012", "3456789")]
        [TestCase("0123", "456789")]
        [TestCase("01234", "56789")]
        [Description("Reattach - Case2 - プレフィクスを再設定するが、その際既存プレフィクスも考慮する")]
        public void Reattach_Case2(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharPosition.BeginningOfFile, prefixText.ToArray(), new StringReader(readerText));
            r.Read();
            r.Read();
            r.Read();

            // Act
            r.Reattach(CharPosition.BeginningOfFile.NextColumn, "ab".ToArray());
            var p = r.Position.Index;
            var l = r.Position.Line;
            var c = r.Position.Column;
            var ch = r.Peek();

            // Assert
            Assert.That(p, Is.EqualTo(1));
            Assert.That(l, Is.EqualTo(1));
            Assert.That(c, Is.EqualTo(2));
            Assert.That(ch, Is.EqualTo('a'));
            Assert.That(r.ReadToEnd(), Is.EqualTo("ab3456789"));
        }
        [Test]
        [Description("ReadToEnd - Case1 - 現在の文字位置から末尾までのテキストを返す")]
        public void ReadToEnd_Case1()
        {
            // Arrange
            var r0 = new PrependableReader(CharPosition.BeginningOfFile, "01234".ToArray(), new StringReader("56789"));
            var r1 = new PrependableReader(CharPosition.BeginningOfFile, "01234".ToArray(), new StringReader("56789"));
            r1.Read();

            // Act
            var s0 = r0.ReadToEnd();
            var s1 = r1.ReadToEnd();

            // Assert
            Assert.That(s0, Is.EqualTo("0123456789"));
            Assert.That(s1, Is.EqualTo("123456789"));
        }
        [Test]
        [Description("ReadToEnd - Case2 - 文字位置がEOFの場合はnullを返す")]
        public void ReadToEnd_Case2()
        {
            // Arrange
            var r0 = new PrependableReader(CharPosition.BeginningOfFile, "01234".ToArray(), new StringReader("56789"));
            var s0 = r0.ReadToEnd();

            // Act
            var s1 = r0.ReadToEnd();

            // Assert
            Assert.That(s1, Is.Null);
        }
        [Test]
        [Description("ReadLine - Case1 - 現在の文字位置から末尾までのテキストを返す")]
        public void ReadLine_Case1()
        {
            // Arrange
            var r0 = new PrependableReader(CharPosition.BeginningOfFile, "012\n34".ToArray(), new StringReader("5\r\n6789"));
            var r1 = new PrependableReader(CharPosition.BeginningOfFile, "012\r\n34".ToArray(), new StringReader("5\n6789"));
            var r2 = new PrependableReader(CharPosition.BeginningOfFile, "012\r\n34".ToArray(), new StringReader("5\n6789"));
            r1.Read();
            r2.Read();
            r2.Read();
            r2.Read();

            // Act
            var s0 = r0.ReadLine();
            var s0_1 = r0.ReadLine();
            var s0_2 = r0.ReadLine();
            var s0_3 = r0.ReadLine();
            var s1 = r1.ReadLine();
            var s1_1 = r1.ReadLine();
            var s1_2 = r1.ReadLine();
            var s1_3 = r1.ReadLine();
            var s2 = r2.ReadLine();

            // Assert
            Assert.That(s0, Is.EqualTo("012"));
            Assert.That(s0_1, Is.EqualTo("345"));
            Assert.That(s0_2, Is.EqualTo("6789"));
            Assert.That(s0_3, Is.Null);
            Assert.That(s1, Is.EqualTo("12"));
            Assert.That(s1_1, Is.EqualTo("345"));
            Assert.That(s1_2, Is.EqualTo("6789"));
            Assert.That(s1_3, Is.Null);
            Assert.That(s2, Is.Empty);
        }
    }
}
