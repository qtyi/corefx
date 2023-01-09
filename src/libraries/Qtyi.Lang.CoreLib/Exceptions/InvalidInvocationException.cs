// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public class InvalidInvocationException : Exception
{
    public InvalidInvocationException() { }

    public InvalidInvocationException(Object? value) : this("value", value) { }

    public InvalidInvocationException(TypeInfo typeInfo) : this("value", typeInfo ?? throw new ArgumentNullException(nameof(typeInfo))) { }

    public InvalidInvocationException(string paramInfo, Object? value) : this(paramInfo, value is null ? TypeInfo.Nil : value.GetType()) { }

    public InvalidInvocationException(string paramInfo, TypeInfo typeInfo) : this($"Attempt to call {paramInfo ?? throw new ArgumentNullException(nameof(paramInfo))} (a {typeInfo ?? throw new ArgumentNullException(nameof(typeInfo))} value)") { }

    public InvalidInvocationException(string? message) : base(message) { }

    public InvalidInvocationException(string? message, System.Exception? innerException) : base(message, innerException) { }

    protected InvalidInvocationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
