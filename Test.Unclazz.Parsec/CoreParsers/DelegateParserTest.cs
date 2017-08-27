﻿using NUnit.Framework;
using System;
using System.Linq;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class DelegateParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Cast<string>();
            var dp = For(r => cp.Parse(r));

            // Act
            var res = dp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture.Present, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Cast<string>();
            var dp = For(r => cp.Parse(r));

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Cast("3210");
            var dp = For(r => cp.Parse(r));

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.True);
            Assert.That(res.Capture.Value, Is.EqualTo("3210"));
        }
    }
}