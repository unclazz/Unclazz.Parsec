using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class CharsWhileInParserTest
    {
        [TestCase("0123456789X", "0", true, 1)]
        [TestCase("0123456789X", "01", true, 2)]
        [TestCase("0123456789X", "10", true, 2)]
        [TestCase("0123456789X", "00", true, 1)]
        [TestCase("0123456789X", "1", false, -1)]
        [TestCase("0123456789X", "12", false, -1)]
        [Description("Parse - Case #1 - 第1引数で指定した文字クラスに該当する一連の文字列はすべてパースすること")]
        public void Parse_Case1(string text, string chars, bool expectedResult, int expectedIndex)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.CharsWhileIn(chars);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(cap.HasValue, Is.False);
                Assert.That(pos.Index, Is.EqualTo(0));
                Assert.That(input.Position.Index, Is.EqualTo(expectedIndex));
            }, (pos, message) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
            });
        }
        [TestCase("0123456789X", "1", true, 0)]
        [TestCase("0123456789X", "0", true, 0)]
        [TestCase("0123456789X", "0", true, 1)]
        [TestCase("0123456789X", "0", false, 2)]
        [TestCase("0123456789X", "01", true, 1)]
        [TestCase("0123456789X", "01", true, 2)]
        [TestCase("0123456789X", "01", false, 3)]
        [Description("Parse - Case #2 - 第2引数minで指定した文字数に満たない場合パースは失敗となること")]
        public void Parse_Case2(string text, string chars, bool expectedResult, int minLength)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.CharsWhileIn(chars, minLength);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(cap.HasValue, Is.False);
                Assert.That(pos.Index, Is.EqualTo(0));
            }, (pos, message) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
            });
        }
    }
}
