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

#endif
