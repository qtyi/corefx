// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Qtyi.Runtime;

public sealed class TypeInfo : Object, IEquatable<TypeInfo>, IEquatable<String>, IEquatable<string>
{
    internal readonly Type? _type;
    private readonly string _name;

    public string Name => this._name;

    public const string TypeInfo_Nil = "nil";
    public const string TypeInfo_Boolean = "boolean";
    public const string TypeInfo_Number = "number";
    public const string TypeInfo_Integer = "integer";
    public const string TypeInfo_Float = "float";
    public const string TypeInfo_String = "string";
    public const string TypeInfo_Function = "function";
    public const string TypeInfo_Userdata = "userdata";
    public const string TypeInfo_Thread = "thread";
    public const string TypeInfo_Table = "dictionary";

    public static readonly TypeInfo Nil = new(BasicType.Nil);
    public static readonly TypeInfo Boolean = new(BasicType.Boolean);
    public static readonly TypeInfo Number = new(BasicType.Number);
    public static readonly TypeInfo Integer = new(BasicType.Integer);
    public static readonly TypeInfo Float = new(BasicType.Float);
    public static readonly TypeInfo String = new(BasicType.String);
    public static readonly TypeInfo Function = new(BasicType.Function);
    public static readonly TypeInfo Userdata = new(BasicType.Userdata);
    public static readonly TypeInfo Thread = new(BasicType.Thread);
    public static readonly TypeInfo Table = new(BasicType.Table);

    internal TypeInfo(string name)
    {
        Debug.Assert(name is not null);
        this._name = name;
        this._type = name switch
        {
            TypeInfo_Nil => null,
            TypeInfo_Boolean => typeof(Boolean),
            TypeInfo_Number or
            TypeInfo_Integer or
            TypeInfo_Float => typeof(Number),
            TypeInfo_String => typeof(String),
            TypeInfo_Function => typeof(Function),
            TypeInfo_Userdata => typeof(Userdata),
            TypeInfo_Thread => typeof(Thread),
            TypeInfo_Table => typeof(Table),
            _ => null
        };
    }

    internal TypeInfo(Type? type)
    {
        this._type = type;
        if (type is null)
            this._name = TypeInfo_Nil;
        else if (type == typeof(Boolean))
            this._name = TypeInfo_Boolean;
        else if (typeof(Number).IsAssignableFrom(type))
        {
            this._type = typeof(Number);
            if (type == typeof(Integer) || type == typeof(LargeInteger))
                this._name = TypeInfo_Integer;
            else if (type == typeof(Real) || type == typeof(DecimalReal))
                this._name = TypeInfo_Float;
            else
                this._name = TypeInfo_Number;
        }
        else if (type == typeof(String))
            this._name = TypeInfo_String;
        else if (type == typeof(Function))
            this._name = TypeInfo_Function;
        else if (type == typeof(Userdata))
            this._name = TypeInfo_Userdata;
        else if (type == typeof(Thread))
            this._name = TypeInfo_Thread;
        else if (type == typeof(Table))
            this._name = TypeInfo_Table;
        else
            this._name = $"'{type.AssemblyQualifiedName}'";
    }

    internal TypeInfo(BasicType type) : this(type switch
    {
        BasicType.Nil => null,
        BasicType.Boolean => typeof(Boolean),
        BasicType.Number => typeof(Number),
        BasicType.String => typeof(String),
        BasicType.Function => typeof(Function),
        BasicType.Userdata => typeof(Userdata),
        BasicType.Thread => typeof(Thread),
        BasicType.Table => typeof(Table),
        _ => throw new NotSupportedException()
    })
    { }

    #region Object
    protected internal override Table? Metatable
    {
        get => Qtyi.Runtime.String.s_mt;
        set => Qtyi.Runtime.String.s_mt = value;
    }

    public override Object? GetMetatable() => Qtyi.Runtime.String.s_mt;

    public override TypeInfo GetTypeInfo() => TypeInfo.String;

    protected override MultiReturns InvokeCore(params Object?[] args) => ((String)this).Invoke(args);
    #endregion

    public override bool Equals(object? obj) => obj switch
    {
        null => false,
        string => this.Equals((string)obj),
        Qtyi.Runtime.String => this.Equals((String)obj),
        TypeInfo => this.Equals((TypeInfo)obj),
        _ => false
    };
    public bool Equals(string? other) => other is not null && this._name == other;
    public bool Equals(String? other) => other is not null && this._name == (string)other;
    public bool Equals(TypeInfo? other)
    {
        if (other is null)
            return false;
        else if (this._type is null && other._type is null)
            return this._name == other._name;
        else
            return this._type == other._type;
    }

    public override int GetHashCode()
    {
        var hashCode = this._name.GetHashCode();
        if (this._type is not null)
            hashCode = HashCode.Combine(hashCode, this._type.GetHashCode());

        return hashCode;
    }

    public override string ToString() => this._name;

    /// <inheritdoc/>
    /// <exception cref="InvalidCastException"><paramref name="type"/> 不是能接受的转换目标类型。</exception>
    public override object ChangeType(Type type) => ((String)this).ChangeType(type);

    public static implicit operator TypeInfo(string type) => new(type ?? throw new ArgumentNullException(nameof(type)));
    public static implicit operator TypeInfo(String type) => new((string)(type ?? throw new ArgumentNullException(nameof(type))));
    public static implicit operator TypeInfo(Type? type) => new(type);

    public static explicit operator string(TypeInfo typeInfo) => typeInfo._name;
    public static explicit operator String(TypeInfo typeInfo) => typeInfo._name;
}

public enum BasicType
{
    Nil,
    Boolean,
    Number,
    Integer,
    Float,
    String,
    Function,
    Userdata,
    Thread,
    Table
}
