// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Qtyi.Lang.CoreLib.UnitTests;

using Qtyi.Runtime;

public abstract class PrimitiveTestBase
{
    protected static readonly Random s_random = new();

    public static IEnumerable<Type> GetPrimitiveTypes() => new[]
    {
        typeof(Boolean),
        typeof(Function),
        typeof(Number),
        typeof(String),
        typeof(Table),
        //typeof(Thread),
        typeof(TypeInfo),
        typeof(Userdata)
    };

    public static Object GetRandomObject()
    {
        var objects = GetRandomObjects().ToArray();
        var count = objects.Length;
        return objects[s_random.Next(count)];
    }

    public static void GetRandomObjects(Object[] buffer) => GetRandomObjects(buffer, 0, buffer.Length);

    public static void GetRandomObjects(Object[] buffer, int index) => GetRandomObjects(buffer, index, buffer.Length - index);

    public static void GetRandomObjects(Object[] buffer, int index, int length)
    {
        Debug.Assert(index >= 0 && index < buffer.Length);
        Debug.Assert(length >= 0 && length <= buffer.Length - index);

        if (length == 0) return;

        var objects = GetRandomObjects().ToArray();
        var count = objects.Length;
        for (var offset = 0; offset < length; offset++)
            buffer[index + offset] = objects[s_random.Next(count)];
    }

    public static IEnumerable<Object> GetRandomObjects(int count = 1) => new[]
    {
        NumberTests.GetRandomNumbers(count).Cast<Object>()
    }.ConcatAll<Object, IEnumerable<Object>>();

}
