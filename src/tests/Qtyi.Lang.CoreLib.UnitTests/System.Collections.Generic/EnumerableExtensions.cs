// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IEnumerable<TItem> ConcatAll<TItem, TCollection>(this IEnumerable<TCollection> source)
        where TCollection : IEnumerable<TItem>
        => source.SelectMany(static x => x);
}
