using System;

namespace ThreePlay.IO.Zero.FCodec {
    /// <summary>
    /// Settings to improve compression. Typically by limited the range of values to support. The same options must be used while writing and reading otherwise corruption will occur.
    /// </summary>
    public sealed class FEncodingOptions {
        /// <summary>
        /// Can nulls be encoded.
        /// </summary>
        /// <example>
        /// Set to FALSE to disable NULLs and improve compression of all values.
        /// </example>
        public bool SupportNulls { get; set; } = true;

        /// <summary>
        /// Set the order of the mapping for booleans. This must be an array of two booleans - TRUE and FALSE, in the order of frequency of occurrence. 
        /// </summary>
        /// <remarks>
        /// If you use TRUE more often, then use { true, false }, if you use FALSE more often then use { false, true }. Setting this correctly will increase the ability to compress value.
        /// </remarks>
        public bool[] BooleanMap { get; set; } = new bool[] { false, true };

        /// <summary>
        /// Set the order of the mapping for nullable booleans. This must be an array of three booleans - TRUE, FALSE and NULL, in the order of frequency of occurrence. 
        /// </summary>
        public bool?[] NullableBooleanMap { get; set; } = new bool?[] { false, true, null };

        /// <summary>
        /// The minimum number to support. 
        /// </summary>
        /// <remarks>
        /// Increasing from 0 to 1 can save a bit per number.
        /// </remarks>
        public ulong UnsignedIntegerMinimum { get; set; } = 0;

        /// <summary>
        /// The amount of accuracy to maintain when compressing integers. Use 1 to maintain all accuracy.
        /// </summary>
        /// <remarks>
        /// A value of 10 means the last digit may be lost. 100 means the last two digits may be lost.
        /// </remarks>
        public ulong IntegerAccuracy { get; set; } = 1;

        /// <summary>
        /// The minimum DateTime value to support.
        /// </summary>
        public DateTime DateTimeMinimum { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        /// <summary>
        /// The amount of accuracy to use when encoding DateTimes. 
        /// </summary>
        public TimeSpan DateTimeAccuracy { get; set; } = new TimeSpan(1);

        /// <summary>
        /// The level of accuracy to use when encoding TimeSpans.
        /// </summary>
        public TimeSpan TimeAccuracy { get; set; } = new TimeSpan(1);

        /// <summary>
        /// The minimum timespan value to support.
        /// </summary>
        public TimeSpan TimeMinimum { get; set; } = new TimeSpan(0);
    }
}
