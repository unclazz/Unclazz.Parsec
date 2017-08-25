using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class ParseResultTest
    {
        [Test]
        public void Successful_Case1()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            var s0 = r0.Successful;
            var s1 = r1.Successful;

            // Assert
            Assert.That(s0, Is.True);
            Assert.That(s1, Is.False);
        }
        [Test]
        public void Value_Case1()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            // Assert
            try
            {
                var v0 = r0.Capture.Value;
                Assert.That(v0, Is.EqualTo(123));
            }
            catch (InvalidOperationException)
            {
                Assert.Fail();
            }
            try
            {
                var v1 = r1.Capture.Value;
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.That(r1.Message, Is.EqualTo("123"));
            }
        }
        [Test]
        public void Message_Case1()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            // Assert
            try
            {
                var v0 = r0.Message;
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.That(r0.Capture.Value, Is.EqualTo(123));
            }
            try
            {
                var v1 = r1.Message;
                Assert.That(v1, Is.EqualTo("123"));
            }
            catch (InvalidOperationException)
            {
                Assert.Fail();
            }
        }
        [Test]
        public void IfSuccessful_Case1()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            // Assert
            r0.IfSuccessful(v => Assert.That(v.Value, Is.EqualTo(123)));
            r1.IfSuccessful(v => Assert.Fail());
        }
        [Test]
        public void IfSuccessful_Case2()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            // Assert
            r0.IfSuccessful(v => Assert.That(v.Value, Is.EqualTo(123)), v => Assert.Fail());
            r1.IfSuccessful(v => Assert.Fail(), v => Assert.That(v, Is.EqualTo("123")));
        }
        [Test]
        public void IfFailed_Case1()
        {
            // Arrange
            var r0 = ParseResult.OfSuccess(CharacterPosition.StartOfFile, 123);
            var r1 = ParseResult.OfFailure<int>(CharacterPosition.StartOfFile, "123");

            // Act
            // Assert
            r0.IfFailed(v => Assert.Fail());
            r1.IfFailed(v => Assert.That(v, Is.EqualTo("123")));
        }
    }
}
