using NUnit.Framework;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class BeginningOfFileParserTest: ParserBase
    {
        [Test]
        public void Parse_Case1()
        {
            var input = Reader.From("hello");
            var parser = BeginningOfFile;
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
        [Test]
        public void Parse_Case2()
        {
            var input = Reader.From("hello");
            var parser = BeginningOfFile;
            input.Read();
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            var input = Reader.From(string.Empty);
            var parser = BeginningOfFile;
            Assert.That(parser.Parse(input).Successful, Is.True);
        }
        [Test]
        public void Parse_Case4()
        {
            var input = Reader.From("hello");
            var parser = BeginningOfFile;
            TestUtility.ConsumesAll(input);
            Assert.That(parser.Parse(input).Successful, Is.False);
        }
    }
}
