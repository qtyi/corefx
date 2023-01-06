// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns<T> : IReadOnlyList<Object?>, IDynamicMetaObjectProvider
    where T : Object
{
    private readonly MultiReturns _values;

    public Object? this[int index] => this._values[index];

    public int Count => this._values.Count;

    public T? Value => (T?)this[0];

    public MultiReturns(T? value) => this._values = new(value);

    #region 解构
    public void Deconstruct(out T? value) => value = this.Value;

    public void Deconstruct(out Object? value) => this._values.Deconstruct(out value);
    #endregion

    #region IReadOnlyList<Object?>
    public IEnumerator<Object?> GetEnumerator() => this._values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    #endregion

    #region IDynamicMetaObjectProvider
    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => ((IDynamicMetaObjectProvider)this._values).GetMetaObject(parameter);
    #endregion

    public static implicit operator MultiReturns(MultiReturns<T> values) => values._values;
}
