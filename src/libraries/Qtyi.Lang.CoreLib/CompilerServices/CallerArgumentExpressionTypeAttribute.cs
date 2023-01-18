// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CallerArgumentExpressionTypeAttribute : Attribute
{
    public string ParameterName { get; }

    public CallerArgumentExpressionTypeAttribute(string parameterName) => this.ParameterName = parameterName;
}

public enum CallerArgumentExpressionType
{
    Field,
    Global
}
