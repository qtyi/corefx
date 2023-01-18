// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

//[assembly: LuaField("table.insert", UnderlyingType = typeof(Table), MemberName = nameof(Table.Insert))]
//[assembly: LuaField("table.pack", UnderlyingType = typeof(Table), MemberName = nameof(Table.Pack))]
//[assembly: LuaField("table.remove", UnderlyingType = typeof(Table), MemberName = nameof(Table.Remove))]
//[assembly: LuaField("table.sort", UnderlyingType = typeof(Table), MemberName = nameof(Table.Sort))]
//[assembly: LuaField("table.unpack", UnderlyingType = typeof(Table), MemberName = nameof(Table.Unpack))]

using Qtyi.Runtime.CompilerServices;

namespace Qtyi.Runtime.Mapping;

[LuaFieldIgnore]
public static class TableModel
{
    [LuaField("table.concat")]
    public static String Concat(Table list, String? sep, Number? i, Number? j)
    {
        sep ??= string.Empty;
        i ??= 0L;
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
}
