// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Lang.CoreLib.UnitTests;

using System.Diagnostics;
using Qtyi.Runtime;

public class NumberTests : PrimitiveTestBase
{
    public static IEnumerable<Number> GetRandomNumbers(int count = 1)
    {
        Debug.Assert(count > 0, "数量必须大于零");

        unchecked
        {
            // Int64
            for (var i = 0; i < count; i++)
            {
                var bytes = new byte[8];
                s_random.NextBytes(bytes);
                yield return (Number)BitConverter.ToInt64(bytes, 0);
            }

            // UInt64
            for (var i = 0; i < count; i++)
            {
                var bytes = new byte[8];
                s_random.NextBytes(bytes);
                yield return (Number)BitConverter.ToInt64(bytes, 0);
            }

            // double
            for (var i = 0; i < count; i++)
            {
                var bytes = new byte[8];
                s_random.NextBytes(bytes);
                yield return (Number)BitConverter.ToDouble(bytes, 0);
            }

            // decimal
            for (var i = 0; i < count; i++)
            {
                var bytes = new byte[8];
                s_random.NextBytes(bytes);
                yield return (Number)(decimal)BitConverter.ToDouble(bytes, 0);
            }
        }
    }
}
