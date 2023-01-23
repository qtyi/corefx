// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Qtyi.Runtime;

[DebuggerDisplay("{_value}")]
public sealed class Boolean : Object, IComparable, IComparable<Boolean>, IComparable<bool>, IEquatable<Boolean>, IEquatable<bool>
{
    private readonly bool _value;

    private Boolean(bool value) => this._value = value;

    public int CompareTo(object? obj)
    {
        var boolean = this.ConvertComparandFrom(obj);

        return this.CompareTo(boolean);
    }

    public int CompareTo(Boolean? other)
    {
        if (other is null) throw new ComparisonNotSupportedException(TypeInfo.TypeInfo_Boolean, TypeInfo.TypeInfo_Nil);

        return this._value.CompareTo(other._value);
    }

    public int CompareTo(bool other) => this._value.CompareTo(other);

    public override bool Equals(object? obj)
    {
        var boolean = this.ConvertComparandFrom(obj);

        return this.Equals(boolean);
    }

    public bool Equals(Boolean? other)
    {
        if (other is null) throw new ComparisonNotSupportedException(TypeInfo.TypeInfo_Boolean, TypeInfo.TypeInfo_Nil);

        return this._value.Equals(other._value);
    }

    public bool Equals(bool other) => this._value.Equals(other);

    public override int GetHashCode() => this._value.GetHashCode();

    /// <summary>
    /// 将操作数从 <see cref="object"/> 转型为 <see cref="Boolean"/> 对象。
    /// </summary>
    /// <exception cref="ComparisonNotSupportedException"><paramref name="obj"/> 的类型不在接受的范围（<see cref="bool"/> 或 <see cref="Boolean"/>）。</exception>
    private Boolean ConvertComparandFrom(object? obj) => obj switch
    {
        bool => (Boolean)(bool)obj,

        Boolean => (Boolean)obj,

        null => throw new ComparisonNotSupportedException(TypeInfo.TypeInfo_Boolean, TypeInfo.TypeInfo_Nil),
        _ => throw new ComparisonNotSupportedException(this.GetTypeInfo(), new TypeInfo(obj?.GetType()))
    };

    #region Object
    internal static Table? s_mt;

    protected internal override Table? Metatable
    {
        get => Boolean.s_mt;
        set => Boolean.s_mt = value;
    }

    protected override String ToStringCore() => this._value.ToString();

    public override TypeInfo GetTypeInfo() => TypeInfo.Boolean;

    /// <inheritdoc/>
    /// <exception cref="InvalidCastException"><paramref name="type"/> 不是能接受的转换目标类型。</exception>
    public override object ChangeType(Type type)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        if (typeof(Object).IsAssignableFrom(type) && type.IsAssignableFrom(typeof(Boolean))) return this;
        else if (type == typeof(bool)) return (bool)this;
        else throw new InvalidCastException();
    }
    #endregion

    #region 操作符
    public static bool operator ==(Boolean left, Boolean right) => left.Equals(right);
    public static bool operator !=(Boolean left, Boolean right) => !left.Equals(right);

    public static implicit operator Boolean(bool value) => new(value);
    public static implicit operator bool(Boolean? value) => value is not null && value._value;
    #endregion
}
