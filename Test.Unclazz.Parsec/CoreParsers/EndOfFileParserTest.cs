using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class EndOfFileParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            var input = ParserInput.FromString("hello");
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
            Assert.That(input.Position.Index, Is.EqualTo(0));
        }
        [Test]
        public void Parse_Case2()
        {
            var input = ParserInput.FromString("h");
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            var input = ParserInput.FromString(string.Empty);
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
        [Test]
        public void Parse_Case4()
        {
            var input = ParserInput.FromString("hello");
            var parser = EndOfFile;
            TestUtility.ConsumesAll(input);
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
    }
}
