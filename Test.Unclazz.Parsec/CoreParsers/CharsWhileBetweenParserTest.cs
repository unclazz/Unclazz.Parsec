using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class CharsWhileBetweenParserTest
    {
        [TestCase("0123456789X", '0', '0', true, 1)]
        [TestCase("0123456789X", '0', '1', true, 2)]
        [TestCase("0123456789X", '0', '2', true, 3)]
        [TestCase("0123456789X", '1', '0', true, 2)]
        [TestCase("0123456789X", '1', '1', false, 0)]
        [TestCase("0123456789X", '1', '2', false, 0)]
        [Description("Parse - Case #1 - 第1引数で指定した文字クラスに該当する一連の文字列はすべてパースすること")]
        public void Parse_Case1(string text, char start, char end, bool expectedResult, int expectedIndex)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = CharsWhileBetween(start, end);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((cap, pos) =>
            {
                Assert.That(cap.Present, Is.False);
                Assert.That(pos.Index, Is.EqualTo(0));
                Assert.That(input.Position.Index, Is.EqualTo(expectedIndex));
            }, (message, pos) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
            });
        }
        [TestCase("0123456789X", '0', '0', true, 1)]
        [TestCase("0123456789X", '0', '1', true, 2)]
        [TestCase("0123456789X", '0', '0', false, 2)]
        [TestCase("0123456789X", '0', '1', false, 3)]
        [Description("Parse - Case #2 - 第2引数minで指定した文字数に満たない場合パースは失敗となること")]
        public void Parse_Case2(string text, char start, char end, bool expectedResult, int minLength)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = CharsWhileBetween(start, end, minLength);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((cap, pos) =>
            {
                Assert.That(cap.Present, Is.False);
                Assert.That(pos.Index, Is.EqualTo(0));
            }, (message, pos) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
            });
        }
    }
}
