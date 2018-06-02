using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class OrParserAndCutParserTest : ParserBase
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var p = Keyword("012A", 2) | Keyword("01B3") | Keyword("0C23");
            Reader i = "0123";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'C'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var p = Keyword("012A", 2) | (Keyword("01B3") | Keyword("0C23"));
            Reader i = "0123";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'A'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var p = Keyword("0123A", 3) | Keyword("012B4", 2) | Keyword("01C34", 1);
            Reader i = "01234";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'C'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case4()
        {
            // Arrange
            var p = Keyword("0123A", 3) | (Keyword("012B4", 2) | Keyword("01C34", 1));
            Reader i = "01234";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'A'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case5()
        {
            // Arrange
            var p = CutStringAt("012A", 2) | Keyword("01B3") | Keyword("0C23");
            Reader i = "0123";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'C'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case6()
        {
            // Arrange
            var p = CutStringAt("012A", 2) | (Keyword("01B3") | Keyword("0C23"));
            Reader i = "0123";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'A'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case7()
        {
            // Arrange
            var p = CutStringAt("0123A", 3) | CutStringAt("012B4", 2) | CutStringAt("01C34", 1);
            Reader i = "01234";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'C'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        [Test]
        public void Parse_Case8()
        {
            // Arrange
            var p = CutStringAt("0123A", 3) | (CutStringAt("012B4", 2) | CutStringAt("01C34", 1));
            Reader i = "01234";

            // Act
            var r = p.Parse(i);

            // Assert
            Assert.That(r.Message, Does.StartWith("expected 'A'"));
            Assert.That(r.CanBacktrack, Is.True);
        }
        Parser CutStringAt(string s, int cutIndex)
        {
            return Keyword(s.Substring(0, cutIndex)).Cut() & Keyword(s.Substring(cutIndex));
        }
    }
}
