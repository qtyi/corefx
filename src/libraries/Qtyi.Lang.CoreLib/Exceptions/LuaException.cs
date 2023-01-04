// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public class LuaException : Exception
{
    public Object? Value { get; }

    public LuaException() : base() { }

    public LuaException(Object value) : this((value ?? throw new ArgumentNullException(nameof(value))).ToString()) => this.Value = Value;

    public LuaException(string? message) : base(message) { }

    public LuaException(string? message, Object value) : base(message) => this.Value = value ?? throw new ArgumentNullException(nameof(value));

    public LuaException(string? message, Exception? innerException) : base(message, innerException) { }

    public LuaException(string? message, Object value, Exception? innerException) : this(message, innerException) => this.Value = value ?? throw new ArgumentNullException(nameof(value));

    protected LuaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        this.Value = (Object?)info.GetValue(nameof(this.Value), typeof(Object));
    }
}
