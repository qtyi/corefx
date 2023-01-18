// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Qtyi.Runtime;

public sealed class MetavalueNotFoundException : Exception
{
    public string MetavalueName => base.Value!.ChangeType<string>();

    public MetavalueNotFoundException(string metavalueName) : this(metavalueName, null) { }

    public MetavalueNotFoundException(string metavalueName, System.Exception? innerException) : base($"metavalue '{metavalueName}' not found", innerException) { }

    public MetavalueNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
