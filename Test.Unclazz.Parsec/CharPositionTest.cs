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
    public class CharPositionTest
    {
        private readonly CharPosition zero; // Not initialized.

        [Test]
        public void ZeroValue_Case1()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(zero.Column, Is.EqualTo(1));
            Assert.That(zero.Index, Is.EqualTo(0));
            Assert.That(zero.Line, Is.EqualTo(1));
        }
        [Test]
        public void ZeroValue_Case2()
        {
            // Arrange
            // Act
            var zeroNextColumn = zero.NextColumn;
            var zeroNextLine = zero.NextLine;

            // Assert
            Assert.That(zeroNextColumn.Column, Is.EqualTo(2));
            Assert.That(zeroNextColumn.Index, Is.EqualTo(1));
            Assert.That(zeroNextColumn.Line, Is.EqualTo(1));
            Assert.That(zeroNextLine.Column, Is.EqualTo(1));
            Assert.That(zeroNextLine.Index, Is.EqualTo(1));
            Assert.That(zeroNextLine.Line, Is.EqualTo(2));
        }
        [Test]
        public void ToString_Case1()
        {
            // Arrange
            // Act
            var zeroString = zero.ToString();
            var zeroNextColumn = zero.NextColumn.ToString();
            var zeroNextLine = zero.NextLine.ToString();
            var zeroNextColumnNextLine = zero.NextColumn.NextLine.ToString();

            // Assert
            Assert.That(zeroString, Is.EqualTo("CharacterPosition(Line = 1, Column = 1, Index = 0)"));
            Assert.That(zeroNextColumn, Is.EqualTo("CharacterPosition(Line = 1, Column = 2, Index = 1)"));
            Assert.That(zeroNextLine, Is.EqualTo("CharacterPosition(Line = 2, Column = 1, Index = 1)"));
            Assert.That(zeroNextColumnNextLine, Is.EqualTo("CharacterPosition(Line = 2, Column = 1, Index = 2)"));
        }
        [Test]
        public void OperatorEqualEqual_Case1()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(zero == CharPosition.BeginningOfFile, Is.True);
            Assert.That(zero.NextColumn == CharPosition.BeginningOfFile.NextColumn, Is.True);
            Assert.That(zero.NextLine == CharPosition.BeginningOfFile.NextLine, Is.True);
        }
    }
}
