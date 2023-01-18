// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T1, T2, T3, T4, T5, T6> : IMultiReturnsInternal
    where T1 : Object
    where T2 : Object
    where T3 : Object
    where T4 : Object
    where T5 : Object
    where T6 : Object
{
    private readonly MultiReturns _values;

    public static MultiReturns<T1, T2, T3, T4, T5, T6> Empty => new(null, null, null, null, null, null);

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T1? Value1 => (T1?)this[0];

    public T2? Value2 => (T2?)this[1];

    public T3? Value3 => (T3?)this[2];

    public T4? Value4 => (T4?)this[3];

    public T5? Value5 => (T5?)this[4];

    public T6? Value6 => (T6?)this[5];

    public MultiReturns(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6) =>
        this._values = new(value1, value2, value3, value4, value5, value6);

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
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    #region 操作符
    public static implicit operator MultiReturns(MultiReturns<T1, T2, T3, T4, T5, T6> values) => values._values;
    public static explicit operator MultiReturns<T1>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1> values) => new(values.Value1, null, null, null, null, null);
    public static explicit operator MultiReturns<T1, T2>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1, values.Value2);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2> values) => new(values.Value1, values.Value2, null, null, null, null);
    public static explicit operator MultiReturns<T1, T2, T3>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1, values.Value2, values.Value3);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2, T3> values) => new(values.Value1, values.Value2, values.Value3, null, null, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1, values.Value2, values.Value3, values.Value4);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2, T3, T4> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, null, null);
    public static explicit operator MultiReturns<T1, T2, T3, T4, T5>(MultiReturns<T1, T2, T3, T4, T5, T6> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5);
    public static implicit operator MultiReturns<T1, T2, T3, T4, T5, T6>(MultiReturns<T1, T2, T3, T4, T5> values) => new(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5, null);
    #endregion
}
