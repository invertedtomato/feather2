using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace InvertedTomato.IO.Feather {
    public static class ArraySegmentExtensions { // TODO: Move into a common lib?
        public static IEnumerable<T> AsEnumerable<T>(this ArraySegment<T> target) {
            return target.Array.Skip(target.Offset).Take(target.Count);
        }

        public static T[] ToArray<T>(this ArraySegment<T> arraySegment) {
            var array = new T[arraySegment.Count];
            Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
            return array;
        }
    }
}
