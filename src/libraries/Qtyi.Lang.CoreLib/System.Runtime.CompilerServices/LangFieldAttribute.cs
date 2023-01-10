// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Module | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate,
    AllowMultiple = true,
    Inherited = false)]
public abstract class LangFieldAttribute : Attribute
{
    public string? QualifiedName { get; init; }

    protected LangFieldAttribute() { }
}

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module,
    AllowMultiple = true,
    Inherited = false)]
public abstract class LangFieldMappingAttribute : Attribute
{
    public string QualifiedName { get; }
    public Type UnderlyingType { get; }

    protected LangFieldMappingAttribute(string qualifiedName, Type underlyingType)
    {
        this.QualifiedName = qualifiedName;
        this.UnderlyingType = underlyingType;
    }
}
