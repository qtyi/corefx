// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MayBeAnyAttribute : Attribute
{
    public string? ParamName { get; set; }
}

[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MayBeAttribute : Attribute
{
    public string? ParamName { get; set; }

    public MayBeAttribute(object? value) => this.Value = value;

    public object? Value { get; }
}

[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public sealed class MayBeNilAttribute : MayBeAttribute
{
    public MayBeNilAttribute() : base(null) { }
}

[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MayBeTypeAttribute : Attribute
{
    public string? ParamName { get; set; }

    public Type Type { get; }

    public MayBeTypeAttribute(Type type) => this.Type = type ?? throw new ArgumentNullException(nameof(type));
}
