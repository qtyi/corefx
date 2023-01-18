// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T1, T2> : IMultiReturnsInternal
    where T1 : Object
    where T2 : Object
{
    private readonly MultiReturns _values;

    public static MultiReturns<T1, T2> Empty => new(null, null);

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T1? Value1 => (T1?)this[0];

    public T2? Value2 => (T2?)this[1];

    public MultiReturns(T1? value1, T2? value2) => this._values = new(value1, value2);

    #region 解构
    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?)"/>
    public void Deconstruct(out T1? value) => value = this.Value1;

    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?, out Object?)"/>
    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = this.Value1;
        value2 = this.Value2;
    }
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    #region 操作符
    public static implicit operator MultiReturns(MultiReturns<T1, T2> values) => values._values;
    public static explicit operator MultiReturns<T1>(MultiReturns<T1, T2> values) => new(values.Value1);
    public static implicit operator MultiReturns<T1, T2>(MultiReturns<T1> values) => new(values.Value1, null);
    #endregion
}
