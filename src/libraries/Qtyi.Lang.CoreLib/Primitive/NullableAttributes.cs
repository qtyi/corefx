// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.CodeAnalysis;

#if !NETCOREAPP

/// <summary>
/// Specifies that null is disallowed as an input even if the corresponding type allows it.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
public sealed class DisallowNullAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the System.Diagnostics.CodeAnalysis.DisallowNullAttribute class.
    /// </summary>
    public DisallowNullAttribute() { }
}

/// <summary>
/// Specifies that the output will be non-null if the named parameter is non-null.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
    /// <summary>
    /// Gets the associated parameter name.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Initializes the attribute with the associated parameter name.
    /// </summary>
    /// <param name="parameterName">The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.</param>
    public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;
}

#endif
