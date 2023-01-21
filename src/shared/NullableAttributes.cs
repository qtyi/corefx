// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices;

#if !NETCOREAPP
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class MaybeNullWhenAttribute : Attribute
{
    public bool ReturnValue { get; }

    public MaybeNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}
#endif
