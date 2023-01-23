// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Lang.CoreLib.UnitTests;

using Qtyi.Runtime;

public class TableTests : PrimitiveTestBase
{
    [Fact]
    public void TableInitializerTest()
    {
        var t = new Table()
        {
            1,
            2,
            3,
            4,
            5,
            { "a", "a" },
            { "b", "b" },
            { "c", "c" },
            { "d", "d" },
            { "e", "e" },
            6, // 尝试添加在第6位，操作完成后列表长度为6
            null, // 尝试添加在第7位，操作完成后列表长度仍为6
            7, // 尝试添加在第7位，操作完成后列表长度为7
            { 8, 7 },
            { 8, 8 }, // 设置新的键值对覆盖了旧的。
        };
        Assert.Equal(1, t[1]);
        Assert.Equal(2, t[2]);
        Assert.Equal(3, t[3]);
        Assert.Equal(4, t[4]);
        Assert.Equal(5, t[5]);
        Assert.Equal("a", t["a"]);
        Assert.Equal("b", t["b"]);
        Assert.Equal("c", t["c"]);
        Assert.Equal("d", t["d"]);
        Assert.Equal("e", t["e"]);
        Assert.Equal(6, t[6]);
        Assert.Equal(7, t[7]);
        Assert.Equal(8, t[8]);
    }
}
