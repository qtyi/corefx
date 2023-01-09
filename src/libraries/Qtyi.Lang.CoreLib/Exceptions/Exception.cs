// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public class Exception : System.Exception
{
    public Object? Value { get; }

    public Exception() : base() { }

    public Exception(Object value) : this((value ?? throw new ArgumentNullException(nameof(value))).ToString()) => this.Value = value;

    public Exception(string? message) : base(message) { }

    public Exception(string? message, Object value) : base(message) => this.Value = value ?? throw new ArgumentNullException(nameof(value));

    public Exception(string? message, System.Exception? innerException) : base(message, innerException) { }

    public Exception(string? message, Object value, System.Exception? innerException) : this(message, innerException) => this.Value = value ?? throw new ArgumentNullException(nameof(value));

    protected Exception(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        this.Value = (Object?)info.GetValue(nameof(this.Value), typeof(Object));
    }
}
