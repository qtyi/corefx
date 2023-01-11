// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Qtyi.CodeAnalysis.UnitTests;

using VerifyAnalyzer = CSharpAnalyzerVerifier<
    LangFieldAttributeDiagnosticAnalyzer,
    DefaultVerifier>;
using VerifyCodeFix = CSharpCodeFixVerifier<
    LangFieldAttributeDiagnosticAnalyzer,
    LangFieldAttributeCodeFixProvider,
    DefaultVerifier>;
using TestAnalyzer = CSharpAnalyzerTest<
    LangFieldAttributeDiagnosticAnalyzer,
    DefaultVerifier>;
using TestCodeFix = CSharpCodeFixTest<
    LangFieldAttributeDiagnosticAnalyzer,
    LangFieldAttributeCodeFixProvider,
    DefaultVerifier>;

public class LangFieldAttributeTests
{
    protected static readonly string AssemblyLocation_QtyiLangCoreLib = typeof(Qtyi.Runtime.Object).Assembly.Location;
    private static readonly ReferenceAssemblies s_frameworkReferenceAssemblies =
#if NET472
        ReferenceAssemblies.NetFramework.Net472.Default;
#elif NETCOREAPP3_1
        ReferenceAssemblies.NetCore.NetCoreApp31;
#elif NET7_0
        ReferenceAssemblies.Net.Net60;
#endif

    private static AttributeTargets? GetAttributeValidOn<T>() where T : Attribute => GetAttributeValidOn(typeof(T));
    private static AttributeTargets? GetAttributeValidOn(Type enumType) => enumType.GetCustomAttribute<AttributeUsageAttribute>()!.ValidOn;

    [Fact]
    public async Task TestAnalyzeNoAttributeUsageAttribute()
    {
        var source = """
            using System.Runtime.CompilerServices;

            internal sealed class SimpleFieldAttribute : LangFieldAttribute
            {
                public SimpleFieldAttribute(string qualifiedName) : base(qualifiedName) { }
            }
            """;

        await new TestAnalyzer()
        {
            ReferenceAssemblies = s_frameworkReferenceAssemblies,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { AssemblyLocation_QtyiLangCoreLib }
            }
        }.RunAsync();
    }

    public static IEnumerable<object[]> SupportedBaseAttributeTypes => new[]
    {
        new object[] { typeof(LangFieldAttribute) },
        new object[] { typeof(LangFieldIgnoreAttribute) },
    };

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestWrongAttributeTarget(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var source =
@"using System;

[AttributeUsage(AttributeTargets.All)]
internal sealed class " + attributeName + " : " + baseAttributeType.FullName + @"
{
" + string.Concat(from ci in baseAttributeType.GetTypeInfo().DeclaredConstructors
                  where !ci.IsPrivate && !ci.IsStatic
                  let args = ci.GetParameters()
                  select $"    public {attributeName}({string.Join(", ", args.Select(arg => $"{arg.ParameterType.FullName} {arg.Name}"))}) : base({string.Join(", ", args.Select(arg => arg.Name))}) {{}}{Environment.NewLine}") + @"}
";

        await new TestAnalyzer()
        {
            ReferenceAssemblies = s_frameworkReferenceAssemblies,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { AssemblyLocation_QtyiLangCoreLib },
                ExpectedDiagnostics =
                {
                    VerifyCodeFix.Diagnostic(LangFieldAttributeDiagnosticAnalyzer.s_UnexpectedAttributeTargets)
                        .WithArguments(
                            baseAttributeType.Name,
                            Enum.Format(typeof(AttributeTargets), GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All, "F")
                        )
                        .WithLocation(3, 17)
                }
            }
        }.RunAsync();
    }
}
