// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T1, T2> : IReadOnlyList<Object?>, IDynamicMetaObjectProvider
    where T1 : Object
    where T2 : Object
{
    private readonly MultiReturns _values;

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T1? Value1 => (T1?)this[0];

    public T2? Value2 => (T2?)this[1];

    public MultiReturns(T1? value1, T2? value2) => this._values = new(value1, value2);

    #region 解构
    public void Deconstruct(out T1? value) => value = this.Value1;

    public void Deconstruct(out Object? value) => this._values.Deconstruct(out value);

    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = this.Value1;
        value2 = this.Value2;
    }

    public void Deconstruct(out Object? value1, out Object? value2) => this._values.Deconstruct(out value1, out value2);
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    #region IDynamicMetaObjectProvider
    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => ((IDynamicMetaObjectProvider)this._values).GetMetaObject(parameter);
    #endregion

    public static implicit operator MultiReturns(MultiReturns<T1, T2> values) => values._values;
}
