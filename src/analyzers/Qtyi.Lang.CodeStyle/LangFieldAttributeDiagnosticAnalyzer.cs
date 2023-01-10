// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CSharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using VisualBasicSyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace Qtyi.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public class LangFieldAttributeDiagnosticAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_MissingAttributeUsageAttribute = new(
        "QTYI1001",
        title: $"缺失“{typeof(AttributeUsageAttribute).FullName}”",
        messageFormat: $"必须指定“{typeof(AttributeUsageAttribute).FullName}”。",
        category: "设计",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor s_UnexpectedAttributeTargets = new(
        "QTYI1002",
        title: $"错误的“{typeof(AttributeTargets).FullName}”",
        messageFormat: "{0}的子类型的AttributeTargets不应在{1}之外。",
        category: "设计",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        s_MissingAttributeUsageAttribute,
        s_UnexpectedAttributeTargets
    );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, CSharpSyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeNode, VisualBasicSyntaxKind.ClassBlock);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var node = context.Node;
        var model = context.SemanticModel;
        if (model.GetDeclaredSymbol(node) is not INamedTypeSymbol symbol) return;

        // 测试是否继承自System.Runtime.CompilerServices.LangFieldAttribute或System.Runtime.CompilerServices.LangFieldMappingAttribute。
        var baseAttributeTypeSymbols = new[]
        {
            model.Compilation.GetTypeByMetadataName(typeof(LangFieldAttribute).FullName),
            model.Compilation.GetTypeByMetadataName(typeof(LangFieldMappingAttribute).FullName)
        };
        INamedTypeSymbol? baseAttributeTypeSymbol;
        for (baseAttributeTypeSymbol = symbol; ; baseAttributeTypeSymbol = symbol.BaseType)
        {
            if (baseAttributeTypeSymbol is null) return;
            else if (baseAttributeTypeSymbols.Contains(baseAttributeTypeSymbol)) break;
        }

        var System_AttributeUsageAttribute_TypeSymbol = model.Compilation.GetTypeByMetadataName(typeof(AttributeUsageAttribute).FullName);
        var attrDatas = symbol.GetAttributes().Where(data => data.AttributeClass == System_AttributeUsageAttribute_TypeSymbol).ToArray();
        if (attrDatas.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                s_MissingAttributeUsageAttribute,
                location: node.GetLocation(),
                additionalLocations: symbol.Locations));
        }
        else
        {
            var expectedTargets = (AttributeTargets?)baseAttributeTypeSymbol.GetAttributes().SingleOrDefault(data => data.AttributeClass == System_AttributeUsageAttribute_TypeSymbol)?.ConstructorArguments[0].Value ?? default;
            var data = attrDatas[0]; // 只检查第一个。

            if (data.ConstructorArguments.Length == 0) return; // 错误的构造函数参数。

            var arg1 = data.ConstructorArguments[0].Value;
            if (arg1 is null || arg1 is not AttributeTargets validOn) return; // 错误的构造函数参数。

            if ((validOn & ~expectedTargets) != 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    s_UnexpectedAttributeTargets,
                    location: node.GetLocation(),
                    additionalLocations: symbol.Locations,
                    baseAttributeTypeSymbol.Name, Enum.Format(typeof(AttributeTargets), expectedTargets, "F")));
            }
        }
    }
}
