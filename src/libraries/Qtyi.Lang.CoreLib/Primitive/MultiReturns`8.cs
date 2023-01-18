// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> : IMultiReturnsInternal
    where T1 : Object
    where T2 : Object
    where T3 : Object
    where T4 : Object
    where T5 : Object
    where T6 : Object
    where T7 : Object
    where TRest : struct, IReadOnlyList<Object?>
{
    private readonly MultiReturns _values;
    private readonly TRest _rest;

    public static MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> Empty => new(null, null, null, null, null, null, null);

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T1? Value1 => (T1?)this[0];

    public T2? Value2 => (T2?)this[1];

    public T3? Value3 => (T3?)this[2];

    public T4? Value4 => (T4?)this[3];

    public T5? Value5 => (T5?)this[4];

    public T6? Value6 => (T6?)this[5];

    public T7? Value7 => (T7?)this[6];

    public TRest Rest => this._rest;

    public MultiReturns(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7, TRest rest) :
        this(new Object?[] { value1, value2, value3, value4, value5, value6, value7 }.Concat(rest).ToArray())
    { }

    public MultiReturns(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7, params Object?[] rest) :
        this(new Object?[] { value1, value2, value3, value4, value5, value6, value7 }.Concat(rest).ToArray())
    { }

    private MultiReturns(Object?[] values)
    {
        this._values = new(values);
        this._rest = MultiReturns.CreateInstance<TRest>(values.Skip(7));
    }

    #region 解构
    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?)"/>
    public void Deconstruct(out T1? value) => value = this.Value1;

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = this.Value1;
        value2 = this.Value2;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
        value4 = this.Value4;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?, out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
        value4 = this.Value4;
        value5 = this.Value5;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?, out Object?, out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
        value4 = this.Value4;
        value5 = this.Value5;
        value6 = this.Value6;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?, out Object?, out Object?, out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
        value4 = this.Value4;
        value5 = this.Value5;
        value6 = this.Value6;
        value7 = this.Value7;
    }

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?, out Object?, out Object?, out Object?, out Object?, out Object?, out MultiReturns)"/>
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7, out TRest rest)
    {
        value1 = this.Value1;
        value2 = this.Value2;
        value3 = this.Value3;
        value4 = this.Value4;
        value5 = this.Value5;
        value6 = this.Value6;
        value7 = this.Value7;
        rest = this.Rest;
    }
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    #region 操作符
    public static implicit operator MultiReturns(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => values._values;
    public static explicit operator MultiReturns<T1>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1> values) => new(values.Value1, null, null, null, null, null, null);
    public static explicit operator MultiReturns<T1, T2>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2> values) => new(values.Value1, values.Value2, null, null, null, null, null);
    public static explicit operator MultiReturns<T1, T2, T3>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2, values.Value3);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2, T3> values) => new(values.Value1, values.Value2, values.Value3, null, null, null, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2, values.Value3, values.Value4);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2, T3, T4> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, null, null, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4, T5>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2, T3, T4, T5> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, null, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, values.Value6);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, values.Value6, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7>(MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, values.Value6, values.Value7);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6, T7, TRest>(MultiReturns<T1, T2, T3, T4, T5, T6, T7> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, values.Value6, values.Value7);
    #endregion
}
