// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System;

internal static class EnumExtensions
{
    public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum
    {
        var enumType = typeof(T);
        if (enumType.GetCustomAttribute<FlagsAttribute>() is null)
            throw new InvalidOperationException($"“{enumType.FullName}”不支持此操作。");

        return ((T[])Enum.GetValues(typeof(T))).Where(flag => value.HasFlag(flag));
    }
}
