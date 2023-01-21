// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public class InvalidLengthOperationException : Exception
{
    public InvalidLengthOperationException() : base() { }

    public InvalidLengthOperationException(string? message) : base(message) { }

    public InvalidLengthOperationException(string? message, System.Exception? innerException) : base(message, innerException) { }

    protected InvalidLengthOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
