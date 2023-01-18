// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Qtyi.CodeAnalysis.CSharp;
using Qtyi.CodeAnalysis.CSharp.Testing.XUnit;

namespace Qtyi.CodeAnalysis.Editor.UnitTests;

using VerifyAnalyzer = AnalyzerVerifier<MultiReturnsRestGenericArgumentDiagnosticAnalyzer>;

public class MultiReturnsRestGenericArgumentTests : TestBase
{
    [Fact]
    public async Task TestGenericNames()
    {
        var source = $$"""
            using System.Collections;
            using System.Collections.Generic;
            using Qtyi.Runtime;

            class Program
            {
                static void Main()
                {
                    var type = typeof(MultiReturns<,,,,,,,>);
                    MultiReturns mr = default;
                    MultiReturns<Object> mr1 = default;
                    MultiReturns<Object, Object> mr2 = default;
                    MultiReturns<Object, Object, Object> mr3 = default;
                    MultiReturns<Object, Object, Object, Object> mr4 = default;
                    MultiReturns<Object, Object, Object, Object, Object> mr5 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object> mr6 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object> mr7 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, {|{{MultiReturnsRestGenericArgumentDiagnosticAnalyzer.UnsupportedTRestType.Id}}:MultiReturns|}> mr8_1 = {|{{MultiReturnsRestGenericArgumentDiagnosticAnalyzer.UnsupportedTRestType.Id}}:default|};
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object>> mr8 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object>> mr9 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object>> mr10 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object, Object>> mr11 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object, Object, Object>> mr12 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object, Object, Object, Object>> mr13 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object, Object, Object, Object, Object>> mr14 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object, Object, Object, Object, Object, Object, Object, MultiReturns<Object>>> mr15 = default;
                    MultiReturns<Object, Object, Object, Object, Object, Object, Object, {|{{MultiReturnsRestGenericArgumentDiagnosticAnalyzer.UnsupportedTRestType.Id}}:S|}> mr8_2 = {|{{MultiReturnsRestGenericArgumentDiagnosticAnalyzer.UnsupportedTRestType.Id}}:default|};
                }
            }

            struct S : IReadOnlyList<Object?>
            {
                public Object? this[int index] => throw null;
                public int Count => throw null;
                public IEnumerator<Object?> GetEnumerator() => throw null;
                IEnumerator IEnumerable.GetEnumerator() => throw null;
            }
            """;

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task TestExpressions()
    {
        var source = $$"""
            using System.Collections;
            using System.Collections.Generic;
            using Qtyi.Runtime;

            class Program
            {
                static void Main()
                {
                    var mr = {|{{MultiReturnsRestGenericArgumentDiagnosticAnalyzer.UnsupportedTRestType.Id}}:GenerateMoreReturns<S>()|};
                }

                static MultiReturns<Object, Object, Object, Object, Object, Object, Object, TRest> GenerateMoreReturns<TRest>(
                    Object? v1 = null,
                    Object? v2 = null,
                    Object? v3 = null,
                    Object? v4 = null,
                    Object? v5 = null,
                    Object? v6 = null,
                    Object? v7 = null,
                    params Object?[] rest)
                    where TRest : struct, IReadOnlyList<Object?>
                    => new MultiReturns<Object, Object, Object, Object, Object, Object, Object, TRest>(v1, v2, v3, v4, v5, v6, v7, rest);
            }

            struct S : IReadOnlyList<Object?>
            {
                public S(params Object?[] values) => throw null;
                public Object? this[int index] => throw null;
                public int Count => throw null;
                public IEnumerator<Object?> GetEnumerator() => throw null;
                IEnumerator IEnumerable.GetEnumerator() => throw null;
            }
            """;

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }
}
