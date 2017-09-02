using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class FlatMapParserTest
    {
        [Test]
        [Description("Parse - Case1 - パース失敗の場合")]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("0123").Capture();
            var mp = kp.FlatMap(a => Yield("abcd"));

            // Act
            var res = mp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case3 - パース成功 x キャプチャありの場合")]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().FlatMap(a => Yield("abcd"));

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("abcd"));
        }
        [Test]
        [Description("Parse - Case4 - パース失敗 x 変換に失敗 x 例外スローNGの場合")]
        public void Parse_Case4()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().FlatMap(a => Yield(bool.Parse(a)));

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case5 - パース失敗 x 変換に失敗 x 例外スローOKの場合")]
        public void Parse_Case5()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().FlatMap(a => Yield(bool.Parse(a)), canThrow: true);

            // Act
            // Assert
            Assert.That(() => mp.Parse("0123XXXX"), Throws.InstanceOf<FormatException>());
        }
        [Test]
        [Description("Parse - Case6 - XMLタグを読み取るパーサーのサンプル")]
        public void Parse_Case6()
        {
            // Arrange
            var angleLeft = Char('<');
            var angleRight = Char('>');
            var leftTag = angleLeft & CharsWhileIn(CharClass.Alphabetic).Capture() & angleRight;
            Func<string,Parser> rightTag = (string s) => angleLeft & Char('/') & Keyword(s) & angleRight;
            var tag = leftTag.FlatMap(rightTag);

            // Act
            var result0 = tag.Parse("<abc></abc>");
            var result1 = tag.Parse("<abc></abd>");

            // Assert
            Assert.That(result0.Successful, Is.True);
            Assert.That(result1.Successful, Is.False);
        }
    }
}
