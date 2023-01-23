// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

using Qtyi.Runtime.CompilerServices;

namespace Qtyi.Runtime.Mapping;

[LuaFieldIgnore]
public static class TableModel
{
    [LuaField("table.concat")]
    public static String Concat(Table list, String? sep, Number? i, Number? j)
    {
        sep ??= string.Empty;
        i ??= 1L;
        j ??= list.Length as Number;

        if (MathModel.Type(i) != TypeInfo.Integer) throw new ArgumentException(i, 3, TypeInfo.Integer);
        if (MathModel.Type(j) != TypeInfo.Integer) throw new ArgumentException(j, 4, TypeInfo.Integer);
        Debug.Assert(sep is not null);
        Debug.Assert(i is not null);
        Debug.Assert(j is not null);

        var values = new List<String>();
        var flag = false;
        for (var index = i; index <= j; index++)
        {
            if (flag)
                values.Add(sep);
            else
                flag = true;

            values.Add(Object.ToString(i));
        }

        return String.Concat(values.ToArray());
    }

    [LuaField("table.insert")]
    public static void Insert(Table list, Object? value) => list.Add(value);

    [LuaField("table.insert")]
    public static void Insert(Table list, long pos, Object? value) => list.Insert(pos, value);

    [LuaField("table.pack")]
    public static Table Pack(params Object?[] values)
    {
        var t = new Table() { { "n", values.Length } };
        t.AddRange(values);
        return t;
    }

    [LuaField("table.remove")]
    public static Object? Remove(Table list)
    {
        if (list.Length is not Number length) throw new InvalidLengthOperationException("object length is not an integer");

        return Remove(list, (long)length.ChangeType(typeof(long)));
    }

    [LuaField("table.remove")]
    public static Object? Remove(Table list, long pos) => list.RemoveAt(pos);

    [LuaField("table.unpack")]
    public static MultiReturns Unpack(Table list, Number? i, Number? j)
    {
        i ??= 1L;
        j ??= list.Length as Number;

        if (MathModel.Type(i) != TypeInfo.Integer) throw new ArgumentException(i, 2, TypeInfo.Integer);
        if (MathModel.Type(j) != TypeInfo.Integer) throw new ArgumentException(j, 3, TypeInfo.Integer);
        Debug.Assert(i is not null);
        Debug.Assert(j is not null);

        long from = (long)i, to = (long)j, length = to - from;
        var values = new Object?[length];
        for (var index = from; index <= to; index++)
        {
            values[index] = list[index];
        }
        return new(values);
    }

    [LuaField("table.sort")]
    public static void Sort(Table list, Function? comp)
    {
        var sorted = ((IEnumerable<Object>)list).OrderBy(static item => item, comp is not null ? new TableSortComparer(comp) : Comparer<Object>.Default).ToArray();
        for (long index = 1L, length = sorted.LongLength; index < length; index++)
        {
            Table.RawSet(list, index, sorted[index]);
        }
    }

    [LuaFieldIgnore]
    internal sealed class TableSortComparer : IComparer<Object>
    {
        private readonly Function _comp;

        public TableSortComparer(Function comp) => this._comp = comp;

        public int Compare(Object? x, Object? y)
        {
            Debug.Assert(x is not null);
            Debug.Assert(y is not null);

            var result = _comp.Invoke(x, y)[0] as Boolean;
            if (result)
                return -1;
            else
                return 0;
        }
    }
}
