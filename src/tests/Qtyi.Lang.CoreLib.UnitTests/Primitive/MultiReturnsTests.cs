// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Lang.CoreLib.UnitTests;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Qtyi.Runtime;

public class MultiReturnsTests : PrimitiveTestBase
{
    /// <summary>
    /// 产生随机的泛型多返回值的类型以及随机的表示多个返回值项的<see cref="Object"/>数组。
    /// </summary>
    /// <remarks>
    /// 产生的类型与数组的项目个数及类型可能不一致，用于测试转型功能。
    /// </remarks>
    public static IEnumerable<object[]> RandomInstanceTypeValuesData
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 1000).Select(_ =>
        {
            MakeData(out var type, out var _);
            MakeData(out var _, out var values);
            return new object[] { type, values };

            static void MakeData(out Type type, out Object?[] values)
            {
                var buffer = new Object[s_random.Next(1, 101)];
                GetRandomObjects(buffer);
                values = buffer;

                type = MultiReturns.MakeInstanceType(0, values);
            }
        });
    }

    /// <summary>
    /// 拆分泛型多返回值的类型，产生泛型类型参数的序列。
    /// </summary>
    /// <param name="type">泛型多返回值的类型。</param>
    /// <returns>拆分<paramref name="type"/>产生泛型类型参数的序列。</returns>
    /// <exception cref="InvalidCastException">类型<paramref name="type"/>不是受支持的多返回值类型。</exception>
    [DebuggerStepThrough]
    private static IEnumerable<Type> FlattenMultiReturnsItemType(Type type)
    {
        var typeDef = type.GetGenericTypeDefinition();
        var typeArgs = type.GenericTypeArguments;

        if (typeDef == typeof(MultiReturns)) return Enumerable.Empty<Type>();
        else if (
            typeDef == typeof(MultiReturns<>) ||
            typeDef == typeof(MultiReturns<,>) ||
            typeDef == typeof(MultiReturns<,,>) ||
            typeDef == typeof(MultiReturns<,,,>) ||
            typeDef == typeof(MultiReturns<,,,,>) ||
            typeDef == typeof(MultiReturns<,,,,,>) ||
            typeDef == typeof(MultiReturns<,,,,,,>))
            return typeArgs;
        else if (typeDef == typeof(MultiReturns<,,,,,,,>))
            return typeArgs.Take(7).Concat(FlattenMultiReturnsItemType(typeArgs[7]));

        throw new InvalidCastException($"类型“{type.FullName}”不是受支持的多返回值类型。");
    }

    /// <summary>
    /// 测试<see cref="MultiReturns.MakeInstanceType(int, Object?[])"/>。
    /// </summary>
    /// <param name="values">随机的<see cref="Object"/>的数组。</param>
    /// <seealso cref="MultiReturns.MakeInstanceType(int, Object?[])"/>
    [Theory]
    [MemberData(nameof(RandomObjectArray), 1000, 1, 100)]
    public void MakeInstanceTypeTest(Object?[] values)
    {
        var expectedTypes = (from value in values select value?.GetPrimitiveType() ?? typeof(Object)).ToArray();
        var actualTypes = FlattenMultiReturnsItemType(MultiReturns.MakeInstanceType(0, values)).ToArray();
        var length = actualTypes.Length;

        Assert.Equal(expectedTypes.Length, length);
        for (var i = 0; i < length; i++)
        {
            Assert.Equal(expectedTypes[i], actualTypes[i]);
        }
    }

    /// <summary>
    /// 测试<see cref="MultiReturns.CreateInstance(Type, int, Object?[])"/>。
    /// </summary>
    /// <param name="type">泛型多返回值的类型。</param>
    /// <param name="values">表示多个返回值项的<see cref="Object"/>数组</param>
    /// <seealso cref="MultiReturns.CreateInstance(Type, int, Object?[])"/>
    [Theory]
    [MemberData(nameof(RandomInstanceTypeValuesData))]
    public void CreateInstanceTest(Type type, Object?[] values)
    {
        dynamic obj = MultiReturns.CreateInstance(type, 0, values);
        // 测试生成的实例的类型是否正确。
        Assert.IsAssignableFrom(type, obj);

        var mr = (MultiReturns)obj;
        // 测试转换为泛型多返回值时是否丢失值。
        var count = mr.Count;
        var length = values.Length;
        Assert.True(count <= length, $"转换为多返回值对象时丢失了{length - count}个值");

        var itemTypes = FlattenMultiReturnsItemType(type).ToArray();
        Debug.Assert(count <= itemTypes.Length);

        // 测试转换为泛型多返回值时各个返回值项的类型是否正确。
        for (int i = 0, maxi = Math.Max(Math.Max(count, itemTypes.Length), length); i < maxi; i++)
        {
            Object? expected;
            if (i >= length) expected = null;
            else if (i >= itemTypes.Length) expected = mr[i];
            else if (values[i] is null || !itemTypes[i].IsAssignableFrom(values[i]!.GetType())) expected = null;
            else expected = values[i];
            var actual = mr[i];
            Assert.Same(expected, actual);
        }
    }

    #region RandomMultiReturns
    public static IEnumerable<object[]> AllRandomMultiReturns
    {
        [DebuggerStepThrough]
        get => new[]
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
    }

    public static IEnumerable<object[]> RandomMultiReturns1
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
            new object[] { MultiReturns.CreateInstance(GetRandomObject()) }
        );
    }

    public static IEnumerable<object[]> RandomMultiReturns2
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[2];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns3
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[3];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns4
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[4];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns5
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[5];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns6
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[6];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns7
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[7];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns8
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[8];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }

    public static IEnumerable<object[]> RandomMultiReturns9
    {
        [DebuggerStepThrough]
        get => Enumerable.Range(0, 100).Select(_ =>
        {
            var values = new Object[9];
            GetRandomObjects(values);
            return new object[] { MultiReturns.CreateInstance(values) };
        });
    }
    #endregion

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
    public void Deconstruct1Test<T1>(MultiReturns<T1> values)
        where T1 : Object
    {
        T1? v1;

        values.Deconstruct(out v1);
        Assert.Same(v1, values[0]);
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns2))]
    public void Deconstruct2Test<T1, T2>(MultiReturns<T1, T2> values)
        where T1 : Object
        where T2 : Object
    {
        T1? v1;
        T2? v2;

        values.Deconstruct(out v1);
        Assert.Same(v1, values[0]);

        (v1, v2) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns3))]
    public void Deconstruct3Test<T1, T2, T3>(MultiReturns<T1, T2, T3> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;

        values.Deconstruct(out v1);
        Assert.Same(v1, values[0]);

        (v1, v2) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);

        (v1, v2, v3) = values;
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns4))]
    public void Deconstruct4Test<T1, T2, T3, T4>(MultiReturns<T1, T2, T3, T4> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;

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
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns5))]
    public void Deconstruct5Test<T1, T2, T3, T4, T5>(MultiReturns<T1, T2, T3, T4, T5> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
        where T5 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;
        T5? v5;

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
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns6))]
    public void Deconstruct6Test<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2, T3, T4, T5, T6> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
        where T5 : Object
        where T6 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;
        T5? v5;
        T6? v6;

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
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns7))]
    public void Deconstruct7Test<T1, T2, T3, T4, T5, T6, T7>(MultiReturns<T1, T2, T3, T4, T5, T6, T7> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
        where T5 : Object
        where T6 : Object
        where T7 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;
        T5? v5;
        T6? v6;
        T7? v7;

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
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns8))]
    public void Deconstruct8Test<T1, T2, T3, T4, T5, T6, T7, T8>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, MultiReturns<T8>> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
        where T5 : Object
        where T6 : Object
        where T7 : Object
        where T8 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;
        T5? v5;
        T6? v6;
        T7? v7;
        MultiReturns<T8> rest;
        T8? v8;

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
        Assert.Same(v1, values[0]);
        Assert.Same(v2, values[1]);
        Assert.Same(v3, values[2]);
        Assert.Same(v4, values[3]);
        Assert.Same(v5, values[4]);
        Assert.Same(v6, values[5]);
        Assert.Same(v7, values[6]);
        rest.Deconstruct(out v8);
        Assert.Same(v8, values[7]);
    }

    [Theory]
    [MemberData(nameof(RandomMultiReturns9))]
    public void Deconstruct9Test<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, MultiReturns<T8, T9>> values)
        where T1 : Object
        where T2 : Object
        where T3 : Object
        where T4 : Object
        where T5 : Object
        where T6 : Object
        where T7 : Object
        where T8 : Object
        where T9 : Object
    {
        T1? v1;
        T2? v2;
        T3? v3;
        T4? v4;
        T5? v5;
        T6? v6;
        T7? v7;
        T8? v8;
        T9? v9;

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
}
