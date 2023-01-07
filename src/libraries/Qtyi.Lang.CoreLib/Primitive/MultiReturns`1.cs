// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T1> : IMultiReturns
    where T1 : Object
{
    private readonly MultiReturns _values;

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T1? Value1 => (T1?)this[0];

    public MultiReturns(T1? value1) => this._values = new(value1);

    #region 解构
    /// <inheritdoc cref="MultiReturns.Deconstruct(out Object?)"/>
    public void Deconstruct(out T1? value1) => value1 = this.Value1;
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    public static implicit operator MultiReturns(MultiReturns<T1> values) => values._values;
}
