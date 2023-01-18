// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns : IMultiReturnsInternal
{
    private readonly int _offset;
    private readonly int _count;
    private readonly Object?[] _values;

    public static MultiReturns Empty => new();

    public Object? this[int index]
    {
        get
        {
            Debug.Assert(index >= 0);

            if (index < this._count)
                return this._values[this._offset + index];
            else
                return null;
        }
    }

    public int Count => this._count;

    private MultiReturns(int offset, Object?[] values)
    {
        Debug.Assert(offset >= 0);
        this._offset = offset;
        this._values = values;
        this._count = 0;
        for (int i = offset, length = values.Length; i < length; i++)
        {
            if (values[i] is not null)
                this._count = i - offset + 1;
        }
    }

    public MultiReturns(params Object?[] values) : this(offset: 0, (Object?[])values.Clone()) { }

    public IEnumerator<Object?> GetEnumerator()
    {
        for (var i = 0; i < this._count; i++)
            yield return this._values[i + this._offset];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #region 解构
    public void Deconstruct(out Object? value1) => value1 = this[0];

    public void Deconstruct(out Object? value1, out Object? value2)
    {
        value1 = this[0];
        value2 = this[1];
    }

    public void Deconstruct(out Object? value1, out Object? value2, out Object? value3)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
    }

    public void Deconstruct(
        out Object? value1,
        out Object? value2,
        out Object? value3,
        out Object? value4)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
        value4 = this[3];
    }

    public void Deconstruct(
        out Object? value1,
        out Object? value2,
        out Object? value3,
        out Object? value4,
        out Object? value5)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
        value4 = this[3];
        value5 = this[4];
    }

    public void Deconstruct(
        out Object? value1,
        out Object? value2,
        out Object? value3,
        out Object? value4,
        out Object? value5,
        out Object? value6)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
        value4 = this[3];
        value5 = this[4];
        value6 = this[5];
    }

    public void Deconstruct(
        out Object? value1,
        out Object? value2,
        out Object? value3,
        out Object? value4,
        out Object? value5,
        out Object? value6,
        out Object? value7)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
        value4 = this[3];
        value5 = this[4];
        value6 = this[5];
        value7 = this[6];
    }

    public void Deconstruct(
        out Object? value1,
        out Object? value2,
        out Object? value3,
        out Object? value4,
        out Object? value5,
        out Object? value6,
        out Object? value7,
        out MultiReturns rest)
    {
        value1 = this[0];
        value2 = this[1];
        value3 = this[2];
        value4 = this[3];
        value5 = this[4];
        value6 = this[5];
        value7 = this[6];
        rest = new(7, this._values);
    }
    #endregion

    public static object CreateInstance(params Object?[] values) =>
        CreateInstance(MakeInstanceType(0, values), 0, (Object?[])values.Clone());

    public static object CreateInstance(IEnumerable<Object?> values)
    {
        var array = values.ToArray();
        return CreateInstance(MakeInstanceType(0, array), 0, array);
    }

    public static T CreateInstance<T>(params Object?[] values) =>
        (T)CreateInstance(typeof(T), 0, (Object?[])values.Clone());

    public static T CreateInstance<T>(IEnumerable<Object?> values) =>
        (T)CreateInstance(typeof(T), 0, values.ToArray());

    internal static object CreateInstance(Type type, int offset, Object?[] values)
    {
        Debug.Assert(offset >= 0);
        if (type == typeof(MultiReturns)) return new MultiReturns(offset, values);
        else if (type.IsGenericType)
        {
            var count = values.Length;
            var typeDef = type.GetGenericTypeDefinition();
            var typeArgs = type.GenericTypeArguments;
            object? rObj = null;
            if (typeDef == typeof(MultiReturns<>))
            {
                var arg1 = getArg(0);
                rObj = Activator.CreateInstance(type, arg1);
            }
            else if (typeDef == typeof(MultiReturns<,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                rObj = Activator.CreateInstance(type, arg1, arg2);
            }
            else if (typeDef == typeof(MultiReturns<,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3);
            }
            else if (typeDef == typeof(MultiReturns<,,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                var arg4 = getArg(3);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3, arg4);
            }
            else if (typeDef == typeof(MultiReturns<,,,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                var arg4 = getArg(3);
                var arg5 = getArg(4);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3, arg4, arg5);
            }
            else if (typeDef == typeof(MultiReturns<,,,,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                var arg4 = getArg(3);
                var arg5 = getArg(4);
                var arg6 = getArg(5);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else if (typeDef == typeof(MultiReturns<,,,,,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                var arg4 = getArg(3);
                var arg5 = getArg(4);
                var arg6 = getArg(5);
                var arg7 = getArg(6);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            else if (typeDef == typeof(MultiReturns<,,,,,,,>))
            {
                var arg1 = getArg(0);
                var arg2 = getArg(1);
                var arg3 = getArg(2);
                var arg4 = getArg(3);
                var arg5 = getArg(4);
                var arg6 = getArg(5);
                var arg7 = getArg(6);
                var rest = CreateInstance(typeArgs[7], 7 + offset, values);
                rObj = Activator.CreateInstance(type, arg1, arg2, arg3, arg4, arg5, arg6, arg7, rest);
            }

            if (rObj is not null) return rObj;

            Object? getArg(int index)
            {
                var i = index + offset;
                if (i >= count) return null;

                var v = values[i];
                if (v is null) return null;
                else if (typeArgs[index].IsAssignableFrom(v.GetType())) return v;
                else return null;
            }
        }

        throw new InvalidCastException($"类型“{type.FullName}”不是受支持的多返回值类型。");
    }

    internal static Type MakeInstanceType(int offset, Object?[] values)
    {
        var count = values.Length - offset;
        Debug.Assert(count > 0);

        var typeArgs = values.Skip(offset).Select(static v => v?.GetPrimitiveType() ?? typeof(Object)).ToArray();
        if (count < 8)
            return (count switch
            {
                1 => typeof(MultiReturns<>),
                2 => typeof(MultiReturns<,>),
                3 => typeof(MultiReturns<,,>),
                4 => typeof(MultiReturns<,,,>),
                5 => typeof(MultiReturns<,,,,>),
                6 => typeof(MultiReturns<,,,,,>),
                7 => typeof(MultiReturns<,,,,,,>),
                _ => throw new InvalidOperationException(),
            }).MakeGenericType(typeArgs);
        else
            return typeof(MultiReturns<,,,,,,,>).MakeGenericType(
                typeArgs[0],
                typeArgs[1],
                typeArgs[2],
                typeArgs[3],
                typeArgs[4],
                typeArgs[5],
                typeArgs[6],
                MakeInstanceType(7 + offset, values));
    }

}
