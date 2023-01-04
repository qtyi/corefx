// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace Qtyi.Runtime;

public readonly struct MultiReturns : IReadOnlyList<Object?>, IDynamicMetaObjectProvider
{
    private readonly Object?[] _values;

    public static MultiReturns Empty => new();

    public Object? this[int index]
    {
        get
        {
            Debug.Assert(index >= 0);

            if (index < this._values.Length) return this._values[index];
            else return null;
        }
    }

    public int Count => this._values.Length;

    public MultiReturns(params Object?[] values) => this._values = values;

    public IEnumerator<Object?> GetEnumerator() => this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
    {
#warning 未实现。
        throw new NotImplementedException();
    }
}
