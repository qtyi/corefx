// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Qtyi.Runtime;

partial class String
{
    internal sealed class Comparer : IComparer<String>
    {
        public static Comparer Default { get; } = new();

        public int Compare(String? x, String? y)
        {
            if (x is null)
            {
                if (y is null) return 0;
                else return -1;
            }
            else if (y is null) return 1;

            var span_x = x._value.Span;
            var span_y = y._value.Span;
            var length = Math.Min(span_x.Length, span_y.Length);
            // 比较每个字节的大小。
            for (var i = 0; i < length; i++)
            {
                var comp = span_x[i].CompareTo(span_y[i]);
                if (comp != 0) return comp;
            }
            // 比较长度的大小。
            return span_x.Length.CompareTo(span_y.Length);
        }
    }

    internal sealed class EqualityComparer : IEqualityComparer<String>
    {
        public static EqualityComparer Default { get; } = new();

        public bool Equals(String? x, String? y)
        {
            if (x is null ^ y is null) return false;
            else if (x is null) return true;

            Debug.Assert(x is not null && y is not null);

            var span_x = x._value.Span;
            var span_y = y._value.Span;
            var length_x = span_x.Length;
            var length_y = span_y.Length;
            if (length_x != length_y) return false;
            var length = Math.Min(length_x, length_y);
            // 比较每个字节的相等性。
            for (var i = 0; i < length; i++)
            {
                var equality = span_x[i] == span_y[i];
                if (!equality) return false;
            }
            return true;
        }

        public int GetHashCode([DisallowNull] String obj)
        {
            int hash = 0;
            var span = obj._value.Span;
            var length = span.Length;
            for (var i = 0; i < length; i++)
            {
                hash = HashCode.Combine(hash, span[i]);
            }
            return hash;
        }
    }
}
