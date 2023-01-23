// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.Lang.CoreLib.UnitTests;

using System.Text.RegularExpressions;
using Qtyi.Runtime;

public class StringTests : PrimitiveTestBase
{
    public static IEnumerable<string> AllUnicodeCharacters
    {
        get
        {
            for (var c = '\u0000'; c <= '\uD7FF'; c++)
                yield return new(new[] { c });

            for (var c = '\uE000'; c <= '\uFFFF'; c++)
            {
                if (c == '\uFFFF') break;
                else
                    yield return new(new[] { c });
            }
            for (var ch = '\uD800'; ch <= '\uD8FF'; ch++)
                for (var cl = '\uDC00'; cl <= '\uDFFF'; cl++)
                    yield return new(new[] { ch, cl });
        }
    }

    public static IEnumerable<byte[]> AllUtf8CharacterByteArrays
    {
        get
        {
            for (byte b = 0x00; b <= 0x7F; b++)
                yield return new[] { b };

            for (byte b1 = 0xC0; b1 <= 0xDF; b1++)
                for (byte b2 = 0x80; b2<= 0xBF; b2++)
                    yield return new[] { b1, b2 };

            for (byte b1 = 0xE0; b1 <= 0xEF; b1++)
                for (byte b2 = 0x80; b2 <= 0xBF; b2++)
                    for (byte b3 = 0x80; b3 <= 0xBF; b3++)
                        yield return new[] { b1, b2, b3 };

            for (byte b1 = 0xF0; b1 <= 0xF7; b1++)
                for (byte b2 = 0x80; b2 <= 0xBF; b2++)
                    for (byte b3 = 0x80; b3 <= 0xBF; b3++)
                        for (byte b4 = 0x80; b4 <= 0xBF; b4++)
                            yield return new[] { b1, b2, b3, b4 };
        }
    }

    public static string RandomUnicodeCharacter
    {
        get
        {
            var i = s_random.Next(0x4F800);
            unchecked
            {
                if (i <= 0xD7FF)
                    return new string(new[] { (char)i });
                else if (i <= 0xF7FF)
                    return new string(new[] { (char)(i + 0x800) });
                else
                {
                    i -= 0xF7FF;
                    var h = i % 0x400;
                    var l = (i - h) / 0x400;
                    return new string(new[] { (char)h, (char)l });
                }
            }
        }
    }

    [Fact]
    public void CharToStringTest()
    {
        foreach (var expected in AllUnicodeCharacters)
        {
            var actual = ((String)expected).ToString();
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void CharsToStringTest()
    {
        var expected = string.Empty;

        Assert.Equal(expected, ((String)expected).ToString());

        for (var i = 0; i < 1000; i++)
        {
            expected = string.Concat(Enumerable.Range(0, s_random.Next(2, 100)).Select(_ => RandomUnicodeCharacter));
            Assert.Equal(expected, ((String)expected).ToString());
        }
    }

    [Fact]
    public void ByteArrayToStringTest()
    {
        foreach (var expected in AllUtf8CharacterByteArrays)
        {
            var s = ((String)expected).ToString();
            byte[] actual;
            Assert.True(s.Length <= 2 || (s.Length % 4 == 0));
            if (s.Length <= 2)
                actual = System.Text.Encoding.UTF8.GetBytes(s);
            else
            {
                actual = new byte[s.Length / 4];
                for (var i = 0; i * 4 < s.Length; i++)
                {
                    var byteString = s.Substring(i * 4, 4);
                    Assert.Matches(@"^\\x[0-9A-Fa-f]{2}$", byteString);
                    var b = byte.Parse(byteString.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    actual[i] = b;
                }
            }
            Assert.True(actual.SequenceEqual(expected));
        }
    }
}
