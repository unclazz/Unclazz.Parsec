using NUnit.Framework;
using System;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class MapParserTest : ParserBase
    {
        [Test]
        [Description("Parse - Case03 - パース成功 x キャプチャありの場合")]
        public void Parse_Case03()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(a => "abcd");

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("abcd"));
        }
        [Test]
        [Description("Parse - Case04 - パース失敗 x 変換に失敗 x 例外スローNGの場合")]
        public void Parse_Case04()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(bool.Parse);

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case05 - パース失敗 x 変換に失敗 x 例外スローOKの場合")]
        public void Parse_Case05()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(bool.Parse, canThrow: true);

            // Act
            // Assert
            Assert.That(() => mp.Parse("0123XXXX"), Throws.InstanceOf<FormatException>());
        }
        [Test]
        [Description("Parse - Case13 - パース成功 x キャプチャありの場合")]
        public void Parse_Case13()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Map(a => "abcd");

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("abcd"));
        }
        [Test]
        [Description("Parse - Case14 - パース失敗 x 変換に失敗 x 例外スローNGの場合")]
        public void Parse_Case14()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Map(bool.Parse);

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case15 - パース失敗 x 変換に失敗 x 例外スローOKの場合")]
        public void Parse_Case15()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Map(bool.Parse, canThrow: true);

            // Act
            // Assert
            Assert.That(() => mp.Parse("0123XXXX"), Throws.InstanceOf<FormatException>());
        }
    }
}
