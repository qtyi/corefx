// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace Qtyi.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public class LangFieldAttributeDiagnosticAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_UnexpectedAttributeTargets = new(
        "QTYI1001",
        title: $"错误的“{typeof(AttributeTargets).FullName}”",
        messageFormat: "{0}的子类型的AttributeTargets不应在{1}之外。",
        category: "设计",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        s_UnexpectedAttributeTargets
    );

    public override void Initialize(AnalysisContext context)
    {
#if false
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeAttributeOperation, OperationKind.Attribute);
    }

    private void AnalyzeAttributeOperation(OperationAnalysisContext context)
    {
        var aOp = (IAttributeOperation)context.Operation;
        if (context.ContainingSymbol is not INamedTypeSymbol symbol) return;

        // 检查是否继承自特定的一些特性类型。
        var compilation = context.Compilation;
        var baseAttributeTypeSymbols = new[]
        {
            compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.LangFieldAttribute"),
            compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.LangFieldIgnoreAttribute")
        };
        INamedTypeSymbol? baseAttributeTypeSymbol;
        for (baseAttributeTypeSymbol = symbol; ; baseAttributeTypeSymbol = baseAttributeTypeSymbol.BaseType)
        {
            if (baseAttributeTypeSymbol is null) return;
            else if (baseAttributeTypeSymbols.Contains(baseAttributeTypeSymbol)) break;
        }

        var System_AttributeUsageAttribute_TypeSymbol = compilation.GetTypeByMetadataName(typeof(AttributeUsageAttribute).FullName);

        // 检查第一个参数（validOn）是否含有值。
        if (aOp.Operation is not IObjectCreationOperation ocOp) return;
        if (ocOp.Constructor?.ContainingSymbol != System_AttributeUsageAttribute_TypeSymbol) return;
        var args = ocOp.Arguments;
        if (args.Length == 0) return;
        var arg1 = args[0].Value;
        var value1 = arg1.ConstantValue;
        if (!value1.HasValue || value1.Value is null || value1.Value.GetType() != Enum.GetUnderlyingType(typeof(AttributeTargets))) return;

        // validOn中的各个标志不能在特定基类规定的应用位置范围之外。
        var validOn = (AttributeTargets)value1.Value;
        var expectedTargets = (AttributeTargets)baseAttributeTypeSymbol.GetAttributes().SingleOrDefault(data => data.AttributeClass == System_AttributeUsageAttribute_TypeSymbol)!.ConstructorArguments[0].Value!;
        if ((validOn & ~expectedTargets) != 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                s_UnexpectedAttributeTargets,
                location: arg1.Syntax.GetLocation(),
                baseAttributeTypeSymbol.Name, Enum.Format(typeof(AttributeTargets), expectedTargets, "F")));
        }
    }
}
