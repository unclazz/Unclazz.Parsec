using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class EndOfFileParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            var input = ParserInput.FromString("hello");
            var parser = Parser.EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
        [Test]
        public void Parse_Case2()
        {
            var input = ParserInput.FromString("h");
            var parser = Parser.EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            var input = ParserInput.FromString(string.Empty);
            var parser = Parser.EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
        [Test]
        public void Parse_Case4()
        {
            var input = ParserInput.FromString("hello");
            var parser = Parser.EndOfFile;
            TestUtility.ConsumesAll(input);
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
    }
}
