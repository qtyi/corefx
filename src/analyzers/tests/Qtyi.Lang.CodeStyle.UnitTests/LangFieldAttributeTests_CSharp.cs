// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Qtyi.CodeAnalysis.CSharp;
using Qtyi.CodeAnalysis.CSharp.Testing.XUnit;

namespace Qtyi.CodeAnalysis.Editor.UnitTests;

using CodeWriter = CSharpCodeWriter;
using VerifyAnalyzer = AnalyzerVerifier<LangFieldAttributeDiagnosticAnalyzer>;
using VerifyCodeFix = CodeFixVerifier<LangFieldAttributeDiagnosticAnalyzer, LangFieldAttributeCodeFixProvider>;

public partial class LangFieldAttributeTests
{
    public static IEnumerable<object[]> SupportedBaseAttributeTypes => new[]
    {
        new object[] { typeof(LangFieldAttribute) },
        new object[] { typeof(LangFieldIgnoreAttribute) },
    };

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestAnalyzeNoAttributeUsageAttribute(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var source = BuildSource<CodeWriter>(writer =>
        {
            writer.Write($$"""
                using System;

                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");
        });

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestAnalyzeAttributeTargets(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var baseAttributeTypeValidOn = GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All;

        var source = BuildSource<CodeWriter>(writer =>
        {
            var validOnStr = string.Join(" | ", baseAttributeTypeValidOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;

                [AttributeUsage({{validOnStr}})]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");
        });

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestAnalyzeSubsetAttributeTargets(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var baseAttributeTypeValidOn = GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All;

        var source = BuildSource<CodeWriter>(writer =>
        {
            var validOn = MakeSubsetAsync(baseAttributeTypeValidOn);
            var validOnStr = string.Join(" | ", validOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;

                [AttributeUsage({{validOnStr}})]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");

            static AttributeTargets MakeSubsetAsync(AttributeTargets targets)
            {
                var flags = targets.GetFlags().ToArray();
                var random = new Random();
                var count = random.Next(1, flags.Length);
                var oldFlags = default(AttributeTargets);
                for (var i = 0; i < count; i++)
                {
                    oldFlags |= flags[random.Next(flags.Length)];
                }

                return targets & ~oldFlags;
            }
        });

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestAnalyzeSupersetAttributeTargets(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var baseAttributeTypeValidOn = GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All;
        if (baseAttributeTypeValidOn == AttributeTargets.All) return;

        var source = BuildSource<CodeWriter>(writer =>
        {
            var validOn = MakeSupersetAsync(baseAttributeTypeValidOn);
            var validOnStr = string.Join(" | ", validOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;

                [AttributeUsage({|{{LangFieldAttributeDiagnosticAnalyzer.s_UnexpectedAttributeTargets.Id}}:{{validOnStr}}|})]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");

            static AttributeTargets MakeSupersetAsync(AttributeTargets targets)
            {
                var complement = ~targets & AttributeTargets.All;
                var flags = complement.GetFlags().ToArray();
                var random = new Random();
                var count = random.Next(1, flags.Length);
                var newFlags = default(AttributeTargets);
                for (var i = 0; i < count; i++)
                {
                    newFlags |= flags[random.Next(flags.Length)];
                }

                return targets | newFlags;
            }
        });

        await VerifyAnalyzer.VerifyAnalyzerAsync(source);
    }

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestFixSupersetAttributeTargetsByFixAttributeArgument(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var baseAttributeTypeValidOn = GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All;
        if (baseAttributeTypeValidOn == AttributeTargets.All) return;

        var source = BuildSource<CodeWriter>(writer =>
        {
            var validOn = MakeSupersetAsync(baseAttributeTypeValidOn);
            var validOnStr = string.Join(" | ", validOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;

                [AttributeUsage({|{{LangFieldAttributeDiagnosticAnalyzer.s_UnexpectedAttributeTargets.Id}}:{{validOnStr}}|},
                    AllowMultiple = false, Inherited = true)]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");

            static AttributeTargets MakeSupersetAsync(AttributeTargets targets)
            {
                var complement = ~targets & AttributeTargets.All;
                var flags = complement.GetFlags().ToArray();
                var random = new Random();
                var count = random.Next(1, flags.Length);
                var newFlags = default(AttributeTargets);
                for (var i = 0; i < count; i++)
                {
                    newFlags |= flags[random.Next(flags.Length)];
                }

                return targets | newFlags;
            }
        });
        var fixedSource = BuildSource<CodeWriter>(writer =>
        {
            var validOnStr = string.Join(" | ", baseAttributeTypeValidOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;
                
                [AttributeUsage({{validOnStr}},
                    AllowMultiple = false, Inherited = true)]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");
        });

        await VerifyCodeFix.VerifyCodeFixAsync(source, fixedSource);
    }

    [Theory]
    [MemberData(nameof(SupportedBaseAttributeTypes))]
    public async Task TestFixSupersetAttributeTargetsByRemoveAttribute(Type baseAttributeType)
    {
        const string attributeName = "SimpleFieldAttribute";
        var baseAttributeTypeValidOn = GetAttributeValidOn(baseAttributeType) ?? AttributeTargets.All;
        if (baseAttributeTypeValidOn == AttributeTargets.All) return;

        var source = BuildSource<CodeWriter>(writer =>
        {
            var validOn = MakeSupersetAsync(baseAttributeTypeValidOn);
            var validOnStr = string.Join(" | ", validOn.GetFlags().Select(flag => "AttributeTargets." + Enum.GetName(typeof(AttributeTargets), flag)));
            writer.WriteLine($$"""
                using System;

                [AttributeUsage({|{{LangFieldAttributeDiagnosticAnalyzer.s_UnexpectedAttributeTargets.Id}}:{{validOnStr}}|})]
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");

            static AttributeTargets MakeSupersetAsync(AttributeTargets targets)
            {
                var complement = ~targets & AttributeTargets.All;
                var flags = complement.GetFlags().ToArray();
                var random = new Random();
                var count = random.Next(1, flags.Length);
                var newFlags = default(AttributeTargets);
                for (var i = 0; i < count; i++)
                {
                    newFlags |= flags[random.Next(flags.Length)];
                }

                return targets | newFlags;
            }
        });
        var fixedSource = BuildSource<CodeWriter>(writer =>
        {
            writer.WriteLine($$"""
                using System;
                
                internal sealed class {{attributeName}} : {{baseAttributeType.FullName}}
                {
                """);
            writer.Indent();
            foreach (var ctor in GenerateCSharpConstructors(attributeName, baseAttributeType))
            {
                writer.WriteLine(ctor);
            }
            writer.Unindent();
            writer.WriteLine("}");
        });

        var test = new CodeFixTest<LangFieldAttributeDiagnosticAnalyzer, LangFieldAttributeCodeFixProvider>();

        await VerifyCodeFix.VerifyCodeFixAsync(source, fixedSource);
    }
}
