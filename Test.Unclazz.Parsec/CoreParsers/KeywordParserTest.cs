using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.CoreParsers;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class KeywordParserTest
    {
        [TestCase("0123456789X", "0", true)]
        [TestCase("0123456789X", "01", true)]
        [TestCase("0123456789X", "1", false)]
        [TestCase("0123456789X", "12", false)]
        [TestCase("0123456789X", "0123456789X", true)]
        [TestCase("0123456789X", "0123456789XY", false)]
        public void Parse_Case1(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
                Assert.That(cap.HasValue, Is.False);
                Assert.That(() => cap.Value, Throws.InstanceOf<InvalidOperationException>());
            });
        }
        [TestCase("0123456789X", "1", true)]
        [TestCase("0123456789X", "12", true)]
        [TestCase("0123456789X", "01", false)]
        public void Parse_Case2(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word);
            input.Read();

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(1));
                Assert.That(cap.HasValue, Is.False);
                Assert.That(() => cap.Value, Throws.InstanceOf<InvalidOperationException>());
            });
        }
        [TestCase("0123456789X", "1", true)]
        [TestCase("0123456789X", "12", true)]
        [TestCase("0123456789X", "01", false)]
        public void Parse_Case3(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word).Capture();
            input.Read();

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(1));
                Assert.That(cap.HasValue, Is.True);
                Assert.That(cap.Value, Is.EqualTo(word));
            });
        }
        [TestCase("0123456789X", "X12", 1, true)]
        [TestCase("0123456789X", "X12", 2, true)]
        [TestCase("0123456789X", "X12", 3, true)]
        [TestCase("0123456789X", "0X2", 1, false)]
        [TestCase("0123456789X", "0X2", 2, true)]
        [TestCase("0123456789X", "0X2", 3, true)]
        [TestCase("0123456789X", "01X", 2, false)]
        [TestCase("0123456789X", "01X", 3, true)]
        [Description("Parse - Case #4 - cutIndexが指すキーワード内の文字位置まえまでマッチが成功していた場合 トラックバックは無効化されること")]
        public void Parse_Case4(string text, string keyword, int cutIndex, bool canBacktrack)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(keyword, cutIndex);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.CanBacktrack, Is.EqualTo(canBacktrack));
        }
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Parse_Case5(int cutIndex)
        {
            // Arrange
            var input = ParserInput.FromString("0123456789X");
            var parser = Parser.Keyword("012X456", cutIndex: cutIndex);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.CanBacktrack, Is.EqualTo(3 < cutIndex));
        }
        [Test]
        public void Constructor_Case1()
        {
            // Arrange
            // Act
            // Assert
            var p0 = new KeywordParser("0123", -1);
            var p2 = new KeywordParser("0123", 1);
            var p3 = new KeywordParser("0123", 3);
        }
        [Test]
        public void Constructor_Case2()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new KeywordParser("0123", -2), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new KeywordParser("0123", 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new KeywordParser("0123", 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
        [Test]
        public void Constructor_Case3()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new KeywordParser("", -1), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => new KeywordParser(null), Throws.InstanceOf<ArgumentNullException>());
        }
        [Test]
        public void ToString_Case1()
        {
            // Arrange
            var parser = new KeywordParser("0123", 2);

            // Act
            var result = parser.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("Keyword(0123, cutIndex = 2)"));
        }
        [Test]
        public void ToString_Case2()
        {
            // Arrange
            var parser = new KeywordParser("0123", -1);

            // Act
            var result = parser.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("Keyword(0123)"));
        }
    }
}
