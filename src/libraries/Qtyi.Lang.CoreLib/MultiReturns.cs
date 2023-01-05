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

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
    {
#warning 未实现。
        throw new NotImplementedException();
    }
}
