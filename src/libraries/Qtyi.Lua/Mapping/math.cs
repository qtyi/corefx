// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

[assembly: LuaField("math.abs", UnderlyingType = typeof(Math), MemberName = nameof(Math.Abs))]
[assembly: LuaField("math.acos", UnderlyingType = typeof(Math), MemberName = nameof(Math.Acos))]
[assembly: LuaField("math.asin", UnderlyingType = typeof(Math), MemberName = nameof(Math.Asin))]
[assembly: LuaField("math.asin", UnderlyingType = typeof(Math), MemberName = nameof(Math.Atan))]
[assembly: LuaField("math.ceil", UnderlyingType = typeof(Math), MemberName = nameof(Math.Ceiling))]
[assembly: LuaField("math.cos", UnderlyingType = typeof(Math), MemberName = nameof(Math.Cos))]
[assembly: LuaField("math.exp", UnderlyingType = typeof(Math), MemberName = nameof(Math.Exp))]
[assembly: LuaField("math.floor", UnderlyingType = typeof(Math), MemberName = nameof(Math.Floor))]
[assembly: LuaField("math.huge", UnderlyingType = typeof(double), MemberName = nameof(double.MaxValue))]
[assembly: LuaField("math.maxinteger ", UnderlyingType = typeof(long), MemberName = nameof(long.MaxValue))]
[assembly: LuaField("math.mininteger ", UnderlyingType = typeof(long), MemberName = nameof(long.MinValue))]
[assembly: LuaField("math.pi ", UnderlyingType = typeof(Math), MemberName = nameof(Math.PI))]
[assembly: LuaField("math.sin ", UnderlyingType = typeof(Math), MemberName = nameof(Math.Sin))]
[assembly: LuaField("math.sqrt ", UnderlyingType = typeof(Math), MemberName = nameof(Math.Sqrt))]
[assembly: LuaField("math.tan ", UnderlyingType = typeof(Math), MemberName = nameof(Math.Tan))]

namespace Qtyi.Runtime.Mapping;

[LuaFieldIgnore]
public static class Math
{
    private static Random s_random = new();

    [LuaField("math.atan")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan(double y, double x = 1D) => System.Math.Atan(y / x);

    [LuaField("math.deg")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Deg(double x) => x * 180D / System.Math.PI;

    [LuaField("math.fmod")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Fmod(long x, long y) => throw new NotImplementedException();

    [LuaField("math.fmod")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Fmod(double x, double y) => throw new NotImplementedException();

    [LuaField("math.max")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Number? Max(Number? x, params Number?[] others)
    {
        var max = x;
        foreach (var y in others)
        {
            if (max < y)
                max = y;
        }
        return max;
    }

    [LuaField("math.min")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Number? Min(Number? x, params Number?[] others)
    {
        var max = x;
        foreach (var y in others)
        {
            if (y < max)
                max = y;
        }
        return max;
    }

    [LuaField("math.modf")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MultiReturns<Number, Number> Modf(double x)
    {
        var integral = System.Math.Ceiling(x);
        var fractional = x - integral;
        return new((long)integral, fractional);
    }

    [LuaField("math.rad")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Rad(double x) => x * System.Math.PI / 180D;

    [LuaField("math.random")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Random() => s_random.Next();

    [LuaField("math.random")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Random(long n)
    {
        if (n == 0) return RandomInt64();

        return Random(1, n);
    }

    [LuaField("math.random")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Random(long m, long n) => throw new NotImplementedException();

    public static (long, long) RandomSeed(long x, long y)
    {
        s_random = new(HashCode.Combine(x, y));
        return (RandomInt64(), RandomInt64());
    }

    private static long RandomInt64()
    {
        var buffer = new byte[8];
        s_random.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }

    [LuaField("math.tointeger")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Number? ToInteger(Number? x)
    {
        if (x is null) return null;

        if (Type(x).Equals("integer")) return x;
        else return (long)x.ChangeType(typeof(long));
    }

    [LuaField("math.type")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeInfo? Type(Number? x) => x?.GetTypeInfo();

    [LuaField("math.ult")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeInfo? UnsignedLessThan(Number? m, Number? n)
    {
        if (m is null) throw new ArgumentException(m, 1, TypeInfo.Integer);
        if (n is null) throw new ArgumentException(n, 1, TypeInfo.Integer);

        throw new NotImplementedException();
    }
}
