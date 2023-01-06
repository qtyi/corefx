// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Lang.CoreLib.UnitTests;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Qtyi.Runtime;

public class MultiReturnsTests : PrimitiveTestBase
{
    public static IEnumerable<object[]> AllRandomMultiReturns => new[]
    {
        RandomMultiReturns1,
        RandomMultiReturns2,
        RandomMultiReturns3,
        RandomMultiReturns4,
        RandomMultiReturns5,
        RandomMultiReturns6,
        RandomMultiReturns7,
        RandomMultiReturns8,
        RandomMultiReturns9,
    }.ConcatAll<object[], IEnumerable<object[]>>();

    public static IEnumerable<object[]> RandomMultiReturns1 => Enumerable.Range(0, 10).Select(_ =>
        new object[] { CreateMultiReturns(GetRandomObject()) }
    );

    public static IEnumerable<object[]> RandomMultiReturns2 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[2];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns3 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[3];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns4 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[4];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns5 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[5];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns6 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[6];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns7 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[7];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns8 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[8];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    public static IEnumerable<object[]> RandomMultiReturns9 => Enumerable.Range(0, 10).Select(_ =>
    {
        var values = new Object[9];
        GetRandomObjects(values);
        return new object[] { CreateMultiReturns(values) };
    });

    internal static object CreateMultiReturns(params Object?[] values)
    {
        var mrType = values.Length switch
        {
            1 => typeof(MultiReturns<>),
            2 => typeof(MultiReturns<,>),
            _ => throw new NotSupportedException()
        };
        var vTypes = values.Select(value => value switch
        {
            Boolean => typeof(Boolean),
            Function => typeof(Function),
            Number => typeof(Number),
            String => typeof(String),
            Table => typeof(Table),
            //Thread => typeof(Thread),
            TypeInfo => typeof(TypeInfo),
            Userdata => typeof(Userdata),
            _ => typeof(Object),
        }).ToArray();
        var type = mrType.MakeGenericType(vTypes);

        var result = Activator.CreateInstance(type, values);
        Debug.Assert(result is not null);

        return result;
    }

    [Theory]
    [MemberData(nameof(AllRandomMultiReturns))]
    public void DeconstructTest(MultiReturns values)
    {
        Object? v1, v2, v3, v4, v5, v6, v7, v8, v9;
        MultiReturns rest;

        values.Deconstruct(out v1);
        Assert.Same(v1, values[0]);

        (v1, v2) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);

        (v1, v2, v3) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);

        (v1, v2, v3, v4) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);

        (v1, v2, v3, v4, v5) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);

        (v1, v2, v3, v4, v5, v6) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);
        Assert.Same(v6, values[5]);

        (v1, v2, v3, v4, v5, v6, v7) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);
        Assert.Same(v6, values[5]);
        Assert.Same(v7, values[6]);

        (v1, v2, v3, v4, v5, v6, v7, rest) = values;
        rest.Deconstruct(out v8);
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);
        Assert.Same(v6, values[5]);
        Assert.Same(v7, values[6]);
        Assert.Same(v8, values[7]);

        (v1, v2, v3, v4, v5, v6, v7, (v8, v9)) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);
        Assert.Same(v6, values[5]);
        Assert.Same(v7, values[6]);
        Assert.Same(v8, values[7]);
        Assert.Same(v9, values[8]);
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns1))]
    public void Deconstruct1Test<T>(MultiReturns<T> values)
        where T : Object
    {
        values.Deconstruct(out T? v1);
        Assert.Same(v1, values[0]);

        values.Deconstruct(out Object? obj1);
        Assert.Same(obj1, values[0]);
    }
}
