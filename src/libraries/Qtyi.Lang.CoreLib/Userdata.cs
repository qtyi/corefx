// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Runtime;

public abstract class Userdata : Object
{
    /// <summary>
    /// 将一个 <see cref="object"/> 对象包装为能够在 Lua 环境中使用的 <see cref="Userdata"/> 对象。
    /// </summary>
    public static Userdata Wrap(object value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        return value is Userdata userdata ? userdata : new Vanilla(value);
    }

    public override TypeInfo GetTypeInfo() => TypeInfo.Userdata;
}
