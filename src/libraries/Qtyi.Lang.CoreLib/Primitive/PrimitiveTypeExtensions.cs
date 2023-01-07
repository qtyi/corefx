// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Runtime;

internal static class PrimitiveTypeExtensions
{
    public static Type? GetPrimitiveType(this Object obj) => obj switch
    {
        Boolean => typeof(Boolean),
        Function => typeof(Function),
        Number => typeof(Number),
        String => typeof(String),
        Table => typeof(Table),
        //Thread => typeof(Thread),
        TypeInfo => typeof(TypeInfo),
        Userdata => typeof(Userdata),
        _ => null,
    };
}
