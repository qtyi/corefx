// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Qtyi.Runtime;

public class ArgumentException : Exception
{
    public ArgumentException(Object? value, int index, Type expectedType, [CallerMemberName] string callerMemberName = "") :
        this(value, index, new TypeInfo(expectedType), callerMemberName)
    { }

    public ArgumentException(Object? value, int index, TypeInfo expectedType, [CallerMemberName] string callerMemberName = "") :
        base($"bad argument #{index} to '{callerMemberName}' ({expectedType} expected, got {value?.GetTypeInfo() ?? TypeInfo.Nil})")
    { }
}
