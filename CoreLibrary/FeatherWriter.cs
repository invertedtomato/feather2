﻿using System;
using System.IO;
using InvertedTomato.Buffers;

namespace ThreePlay.IO.Feather {
    public class FeatherWriter : IDisposable {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private readonly Options Options;
        private readonly Stream Output;


        public FeatherWriter(Stream output) : this(output, new Options()) { }

        public FeatherWriter(Stream output, Options options) {
#if DEBUG
            if (null == output) {
                throw new ArgumentNullException("output");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Store
            Output = output;
            Options = options;
        }

        public void Write(IEncoder payload) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
#endif

            Write(new IEncoder[] { payload });
        }

        public void Write(IEncoder[] payloads) {
#if DEBUG
            if (null == payloads) {
                throw new ArgumentNullException("payloads");
            }
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
#endif

            // For each payload...
            foreach (var payload in payloads) {
#if DEBUG
                if (null == payload) {
                    throw new ArgumentNullException("payloads", "Element in array.");
                }
#endif

                // Encode payload
                var buffer = payload.GetBuffer();

                // Write to output
                Output.Write(buffer);
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
            }

            // Set large fields to null
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
