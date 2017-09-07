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
    public class SeqTest
    {
        [TestCase(new int[] { })]
        [TestCase(new int[] { 0 })]
        [TestCase(new int[] { 0, 1, 2 })]
        public void Of_ReturnsNewInstance(int[] items)
        {
            // Arrange
            // Act
            var seq = Seq<int>.Of(items);

            // Assert
            Assert.That(seq.ToArray(), Is.EqualTo(items));
        }
        [Test]
        public void Of_ThrowsAppliedToNull()
        {
            // Arrange
            int[] items = null;

            // Act
            // Assert
            Assert.That(() => Seq<int>.Of(items), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Plus_Seq_Seq__ReturnsSeqConsistsOf2SeqsElements()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var seq1 = Seq<char>.Of("345");

            // Act
            var seq2 = seq0 + seq1;

            // Assert
            Assert.That(seq2.ToArray(), Is.EqualTo("012345".ToArray()));
        }
        [Test]
        public void Plus_Seq_Seq__ReturnsSeqConsistsOf2SeqsElements_WhenLeftIsEmpty()
        {
            // Arrange
            var seq0 = Seq<char>.Empty;
            var seq1 = Seq<char>.Of("345");

            // Act
            var seq2 = seq0 + seq1;

            // Assert
            Assert.That(seq2.ToArray(), Is.EqualTo("345".ToArray()));
        }
        [Test]
        public void Plus_Seq_Enumerable__ReturnsSeqConsistsOf2CollectionsElements_WhenLeftIsCollection()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var col0 = new Queue<char>("345");

            // Act
            var seq1 = col0 + seq0;

            // Assert
            Assert.That(seq1.ToArray(), Is.EqualTo("345012".ToArray()));
        }
        [Test]
        public void Plus_Seq_Enumerable__ReturnsSeqConsistsOf2CollectionsElements_WhenRightIsCollection()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var col0 = new Queue<char>("345");

            // Act
            var seq1 = seq0 + col0;

            // Assert
            Assert.That(seq1.ToArray(), Is.EqualTo("012345".ToArray()));
        }
        [Test]
        public void Plus_Seq_Elem__ReturnsNewSeqWithSpecifiedItem_WhenRightIsNewItem()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var ch0 = '3';

            // Act
            var seq1 = seq0 + ch0;

            // Assert
            Assert.That(seq1.ToArray(), Is.EqualTo("0123".ToArray()));
        }
        [Test]
        public void Plus_Seq_Elem__ReturnsNewSeqWithSpecifiedItem_WhenLeftIsNewItem()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var ch0 = '3';

            // Act
            var seq1 = ch0 + seq0;

            // Assert
            Assert.That(seq1.ToArray(), Is.EqualTo("3012".ToArray()));
        }
        [Test]
        public void Plus_Seq_Seq__ReturnsSeqConsistsOf2SeqsElements_WhenRightIsEmpty()
        {
            // Arrange
            var seq0 = Seq<char>.Of("012");
            var seq1 = Seq<char>.Empty;

            // Act
            var seq2 = seq0 + seq1;

            // Assert
            Assert.That(seq2.ToArray(), Is.EqualTo("012".ToArray()));
        }
        [Test]
        public void ToString_Test()
        {
            // Arrange
            var seq0 = Seq<int>.Empty;
            var seq1 = Seq<int>.Of(1);
            var seq2 = Seq<int>.Of(1, 2, 3);

            // Act
            // Assert
            Assert.That(seq0.ToString(), Is.EqualTo("[]"));
            Assert.That(seq1.ToString(), Is.EqualTo("[1]"));
            Assert.That(seq2.ToString(), Is.EqualTo("[1, 2, 3]"));
        }
        [TestCase(new int[] { })]
        [TestCase(new int[] { 0 })]
        [TestCase(new int[] { 0, 1, 2 })]
        public void Indexer_ReturnsElementAtSpecifiedIndex(int[] items)
        {
            // Arrange
            // Act
            var seq = Seq<int>.Of(items);

            // Assert
            for (var i = 0; i < items.Length; i++)
            {
                Assert.That(seq[i], Is.EqualTo(items[i]));
            }
        }
        [TestCase(new int[] { })]
        [TestCase(new int[] { 0 })]
        [TestCase(new int[] { 0, 1, 2 })]
        public void Indexer_ThrowsExceptionIfSpecifiedIndexIsOutOfBounds(int[] items)
        {
            // Arrange
            // Act
            var seq = Seq<int>.Of(items);

            // Assert
            Assert.That(() => seq[-1], Throws.InstanceOf<IndexOutOfRangeException>());
            Assert.That(() => seq[items.Length], Throws.InstanceOf<IndexOutOfRangeException>());
        }
        [Test]
        public void Map_ReturnsNewSeqConsistsOfTransformedItems()
        {
            // Arrange
            var seq0 = Seq<int>.Of(0, 1, 2);

            // Act
            var seq1 = seq0.Map(i => i.ToString());

            // Assert
            Assert.That(seq1.ToArray(), Is.EqualTo(new[] { "0", "1", "2" }));
        }
    }
}
