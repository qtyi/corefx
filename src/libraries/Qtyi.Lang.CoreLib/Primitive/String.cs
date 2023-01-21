// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Qtyi.Runtime;

public sealed partial class String : Object, IComparable, IComparable<String?>, IComparable<string>, IEquatable<String>, IEquatable<string>, ICloneable, IConvertible
{
    private readonly ReadOnlyMemory<byte> _value;

    private String(ReadOnlyMemory<byte> value) => this._value = value;
    private String(string value) => this._value = Encoding.UTF8.GetBytes(value);

    static String()
    {
#warning 设置字符串数据类型的默认元表。
    }

    public int CompareTo(object? obj) => obj switch
    {
        null or String => this.CompareTo((String?)obj),
        _ => this.CompareTo(obj.ToString())
    };

    public int CompareTo(String? other) => Comparer.Default.Compare(this, other);

    public int CompareTo(string? other) => this.CompareTo(other is null ? null : (String)other);

    public override bool Equals(object? obj) => obj switch
    {
        null or String => this.Equals((String?)obj),
        _ => this.Equals(obj.ToString())
    };

    public bool Equals(String? other) => EqualityComparer.Default.Equals(this, other);

    public bool Equals(string? other) => this.Equals(other is null ? null : (String)other);

    public static String Concat(params String[] values)
    {
        var length = values.Sum(value => value._value.Length);
        var newValue = new byte[length];
        int position = 0;
        foreach (var value in values)
        {
            value._value.Span.CopyTo(newValue.AsSpan(position));
            position += value._value.Length;
        }

        return newValue;
    }

    #region ICloneable
    public object Clone()
    {
        Memory<byte> newValue = new byte[this._value.Length];
        this._value.CopyTo(newValue);
        return new String(newValue);
    }
    #endregion

    #region IConvertible
    bool IConvertible.ToBoolean(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToBoolean(provider);

    sbyte IConvertible.ToSByte(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToSByte(provider);

    byte IConvertible.ToByte(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToByte(provider);

    short IConvertible.ToInt16(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToInt16(provider);

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToUInt16(provider);

    int IConvertible.ToInt32(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToInt32(provider);

    uint IConvertible.ToUInt32(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToUInt32(provider);

    long IConvertible.ToInt64(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToInt64(provider);

    ulong IConvertible.ToUInt64(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToUInt64(provider);

    float IConvertible.ToSingle(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToSingle(provider);

    double IConvertible.ToDouble(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToDouble(provider);

    decimal IConvertible.ToDecimal(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToDecimal(provider);

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToDateTime(provider);

    char IConvertible.ToChar(IFormatProvider? provider) => ((IConvertible)this.ToString()).ToChar(provider);

    public string ToString(IFormatProvider? provider) => this.ToString().ToString(provider);

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => this.ChangeType(conversionType);

    TypeCode IConvertible.GetTypeCode() => this.ToString().GetTypeCode();
    #endregion

    #region Object
    internal static Table? s_mt;

    protected internal override Table? Metatable
    {
        get => String.s_mt;
        set => String.s_mt = value;
    }

    /// <summary>
    /// 获取字符串的字节长度。
    /// </summary>
    public override Object? Length => this._value.Length;

    public override int GetHashCode() => this._value.GetHashCode();

    public override string ToString() => Encoding.UTF8.GetString(this._value.ToArray());

    protected override String ToStringCore() => this;

    public override TypeInfo GetTypeInfo() => TypeInfo.String;

    /// <inheritdoc/>
    /// <exception cref="InvalidCastException"><paramref name="type"/> 不是能接受的转换目标类型。</exception>
    public override object ChangeType(Type type)
    {
        if (typeof(Object).IsAssignableFrom(type) && type.IsAssignableFrom(typeof(String))) return this;
        else if (type == typeof(string)) return this._value;
        else return ((IConvertible)this.ToString()).ToType(type, null);
    }
    #endregion

    #region 操作符
    public static bool operator ==(String left, String right) => left.Equals(right);
    public static bool operator !=(String left, String right) => !left.Equals(right);

    public static implicit operator String(byte[] value) => new((byte[])value.Clone());
    public static implicit operator String(string value) => new(value);
    public static explicit operator byte[](String str) => str._value.ToArray();
    public static explicit operator string(String str) => str.ToString();
    #endregion
}
