// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public class ComparisonNotSupportedException : Exception
{
    public ComparisonNotSupportedException(string left, string right) : this($"attempt to compare {left ?? throw new ArgumentNullException(nameof(left))} with {right ?? throw new ArgumentNullException(nameof(right))}") { }

    public ComparisonNotSupportedException(TypeInfo left, TypeInfo right) : this((left ?? throw new ArgumentNullException(nameof(left))).ToString(), (right ?? throw new ArgumentNullException(nameof(right))).ToString()) { }

    public ComparisonNotSupportedException(string? message) : base(message) { }

    public ComparisonNotSupportedException(string? message, System.Exception? innerException) : base(message, innerException) { }

    protected ComparisonNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
