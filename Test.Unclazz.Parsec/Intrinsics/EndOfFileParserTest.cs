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
    public class EndOfFileParserTest: ParserBase
    {
        [Test]
        public void Parse_Case1()
        {
            var input = Reader.From("hello");
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
            Assert.That(input.Position.Index, Is.EqualTo(0));
        }
        [Test]
        public void Parse_Case2()
        {
            var input = Reader.From("h");
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            var input = Reader.From(string.Empty);
            var parser = EndOfFile;
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
        [Test]
        public void Parse_Case4()
        {
            var input = Reader.From("hello");
            var parser = EndOfFile;
            TestUtility.ConsumesAll(input);
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
    }
}
