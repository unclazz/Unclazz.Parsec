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
            var input = Reader.From(text);
            var parser = CharsWhileIn(chars);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful(() =>
            {
                Assert.That(input.Position.Index, Is.EqualTo(expectedIndex));
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
            var input = Reader.From(text);
            var parser = CharsWhileIn(chars, minLength);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
        }
    }
}
