// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate,
    AllowMultiple = true,
    Inherited = false)]
public abstract class LangFieldAttribute : Attribute
{
    public string QualifiedName { get; }
    public string? MemberName { get; init; }
    public Type? UnderlyingType { get; init; }

    protected LangFieldAttribute(string qualifiedName) => this.QualifiedName = qualifiedName;
}

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate,
    AllowMultiple = true,
    Inherited = false)]
public abstract class LangFieldIgnoreAttribute : Attribute
{
    public string? MemberName { get; init; }
    public Type? UnderlyingType { get; init; }

    protected LangFieldIgnoreAttribute() { }
}
