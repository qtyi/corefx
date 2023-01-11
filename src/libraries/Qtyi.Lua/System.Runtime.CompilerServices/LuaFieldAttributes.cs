// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices;

internal sealed class LuaFieldAttribute : LangFieldAttribute
{
    public LuaFieldAttribute(string qualifiedName) : base(qualifiedName) { }
}

internal sealed class LuaFieldIgnoreAttribute : LangFieldIgnoreAttribute
{
    public LuaFieldIgnoreAttribute() : base() { }
}
