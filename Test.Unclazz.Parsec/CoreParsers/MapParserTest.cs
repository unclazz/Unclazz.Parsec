using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class MapParserTest
    {
        [Test]
        [Description("Parse - Case3 - パース成功 x キャプチャありの場合")]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(a => "abcd");

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("abcd"));
        }
        [Test]
        [Description("Parse - Case4 - パース失敗 x 変換に失敗 x 例外スローNGの場合")]
        public void Parse_Case4()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(bool.Parse);

            // Act
            var res = mp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Value, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case5 - パース失敗 x 変換に失敗 x 例外スローOKの場合")]
        public void Parse_Case5()
        {
            // Arrange
            var kp = Keyword("0123");
            var mp = kp.Capture().Map(bool.Parse, canThrow: true);

            // Act
            // Assert
            Assert.That(() => mp.Parse("0123XXXX"), Throws.InstanceOf<FormatException>());
        }
    }
}
