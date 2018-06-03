using System;
using NUnit.Framework;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class SkipToParserTest : ParserBase
    {
        [Test]
        public void Parse_Case1()
        {
            var result = SkipTo("*/").Parse("/**/abc");

            Assert.That(result.Successful, Is.True);
            Assert.That(result.End.Index, Is.EqualTo(4));
        }

        [Test]
        public void Parse_Case2()
        {
            var result = SkipTo("*/").Parse("/* */abc");

            Assert.That(result.Successful, Is.True);
            Assert.That(result.End.Index, Is.EqualTo(5));
        }

        [Test]
        public void Parse_Case3()
        {
            var result = SkipTo("*/").Parse("/*  */abc");

            Assert.That(result.Successful, Is.True);
            Assert.That(result.End.Index, Is.EqualTo(6));
        }

        [Test]
        public void Parse_Case4()
        {
            var result = SkipTo("*/").Parse("/* * */abc");

            Assert.That(result.Successful, Is.True);
            Assert.That(result.End.Index, Is.EqualTo(7));
        }

        [Test]
        public void Parse_Case5()
        {
            var result = SkipTo("*/").Parse("/* *abc");

            Assert.That(result.Successful, Is.False);
            Assert.That(result.End.Index, Is.EqualTo(7));
        }
    }
}
