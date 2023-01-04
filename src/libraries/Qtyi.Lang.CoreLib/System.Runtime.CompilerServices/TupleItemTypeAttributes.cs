// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public sealed class TupleItemTypeAttribute : Attribute
{
    public int ItemIndex { get; }
    public Type? ItemType { get; }

    public TupleItemTypeAttribute(int itemIndex, Type itemType)
    {
        this.ItemIndex = itemIndex;
        this.ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
    }
}

[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public sealed class TupleItemTypeSameAsAttribute : Attribute
{
    public int ItemIndex { get; }
    public string ParamName { get; }

    public TupleItemTypeSameAsAttribute(int itemIndex, string paramName)
    {
        this.ItemIndex = itemIndex;
        this.ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
    }
}
