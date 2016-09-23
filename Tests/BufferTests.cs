using InvertedTomato.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace InvertedTomato.Tests {
    [TestClass]
    public class BufferTests {
        [TestMethod]
        public void Init_MaxCapacity() {
            var buffer = new Buffer<byte>(2);

            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(0, buffer.End);
            Assert.AreEqual(0, buffer.Used);
            Assert.AreEqual(2, buffer.Available);
            Assert.AreEqual(2, buffer.MaxCapacity);
            Assert.AreEqual(false, buffer.IsFull);
            Assert.AreEqual(true, buffer.IsEmpty);
        }

        [TestMethod]
        public void Init_Array() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        public void Init_ArraySubset() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 }, 1);

            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(1, buffer.End);
            Assert.AreEqual(2, buffer.MaxCapacity);
        }

        [TestMethod]
        public void Init_ArraySuperset() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 }, 4);

            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
            Assert.AreEqual(4, buffer.MaxCapacity);
        }

        [TestMethod]
        public void Enqueue_Single() {
            var buffer = new Buffer<byte>(2);

            // First enqueue
            buffer.Enqueue(1);

            // Check underlying
            var underlying = buffer.GetUnderlying();
            Assert.AreEqual(2, underlying.Length);
            Assert.AreEqual(1, underlying[0]);
            Assert.AreEqual(0, underlying[1]);

            // Get others
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(1, buffer.End);
            Assert.AreEqual(1, buffer.Used);
            Assert.AreEqual(1, buffer.Available);
            Assert.AreEqual(2, buffer.MaxCapacity);
            Assert.AreEqual(false, buffer.IsFull);
            Assert.AreEqual(false, buffer.IsEmpty);

            // Second enqueue
            buffer.Enqueue(1);

            // Check underlying
            underlying = buffer.GetUnderlying();
            Assert.AreEqual(2, underlying.Length);
            Assert.AreEqual(1, underlying[0]);
            Assert.AreEqual(1, underlying[1]);

            // Check others
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
            Assert.AreEqual(2, buffer.Used);
            Assert.AreEqual(0, buffer.Available);
            Assert.AreEqual(2, buffer.MaxCapacity);
            Assert.AreEqual(true, buffer.IsFull);
            Assert.AreEqual(false, buffer.IsEmpty);
        }

        [TestMethod]
        public void Enqueue_Buffer() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 }, 10);

            buffer.Enqueue(buffer);
            Assert.AreEqual(4, buffer.Used);

        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Enqueue_Overflow() {
            var buffer = new Buffer<byte>(2);
            buffer.Enqueue(1);
            buffer.Enqueue(2);
            buffer.Enqueue(3);
        }

        [TestMethod]
        public void Dequeue() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(1, buffer.Dequeue());
            Assert.AreEqual(1, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(2, buffer.Dequeue());
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Dequeue_Overflow() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(1, buffer.Dequeue());
            Assert.AreEqual(2, buffer.Dequeue());
            buffer.Dequeue();
        }

        [TestMethod]
        public void TryDequeue() {
            byte output;

            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(true, buffer.TryDequeue(out output));
            Assert.AreEqual(1, output);
            Assert.AreEqual(1, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(true, buffer.TryDequeue(out output));
            Assert.AreEqual(2, output);
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(false, buffer.TryDequeue(out output));
            Assert.AreEqual(0, output);
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(false, buffer.TryDequeue(out output));
            Assert.AreEqual(0, output);
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        public void Peek() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(1, buffer.Peek());
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(1, buffer.Peek());
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Peek_Overflow() {
            var buffer = new Buffer<byte>(0);

            buffer.Peek();
        }

        [TestMethod]
        public void TryPeek() {
            byte value;

            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(true, buffer.TryPeek(out value));
            Assert.AreEqual(1, value);
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(true, buffer.TryPeek(out value));
            Assert.AreEqual(1, value);
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        public void TryPeek_Overflow() {
            byte value;

            var buffer = new Buffer<byte>(0);

            Assert.AreEqual(false, buffer.TryPeek(out value));
        }

        [TestMethod]
        public void Peek_Absolute() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            Assert.AreEqual(1, buffer.Peek(0));
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);

            Assert.AreEqual(2, buffer.Peek(1));
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.End);
        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Peek_Absolute_Overflow() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });
            buffer.Peek(3);
        }

        [TestMethod]
        public void Resize_Start() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });
            var buffer2 = buffer.Resize(8);
            Assert.AreEqual(buffer.Start, buffer2.Start);
            Assert.AreEqual(buffer.End, buffer2.End);
            Assert.AreEqual(buffer.Peek(), buffer2.Peek());
        }

        [TestMethod]
        public void Resize_StartPlusOne() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });
            buffer.Dequeue();

            var buffer2 = buffer.Resize(8);
            Assert.AreEqual(0, buffer2.Start);
            Assert.AreEqual(1, buffer2.End);
            Assert.AreEqual(buffer.Peek(), buffer2.Peek());
        }

        [TestMethod]
        public void Resize_AlmostOverflow() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });
            buffer.Resize(2);
        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Resize_Overflow() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });
            buffer.Resize(1);
        }

        [TestMethod]
        public void Enumerate() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 });

            var enumerator = ((IEnumerable<byte>)buffer).GetEnumerator();
            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.AreEqual(false, enumerator.MoveNext());
        }

        [TestMethod]
        public void IncremenentEnd() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 }, 4);
            buffer.IncrementEnd(2);
            Assert.AreEqual(4, buffer.End);
        }

        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void IncremenentEnd_Overflow() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2 }, 4);
            buffer.IncrementEnd(3);
        }

        [TestMethod]
        public void ToArray() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2, 3 });
            buffer.Dequeue();

            var result = buffer.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [TestMethod]
        public void Reset() {
            var buffer = new Buffer<byte>(new byte[] { 1, 2, 3 }, 4);
            buffer.Dequeue();
            buffer.Reset();

            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(0, buffer.End);
        }
    }
}
