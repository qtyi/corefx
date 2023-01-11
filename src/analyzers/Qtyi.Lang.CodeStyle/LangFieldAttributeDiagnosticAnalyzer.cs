// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Qtyi.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class LangFieldAttributeDiagnosticAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor s_UnexpectedAttributeTargets = new(
        "QTYI1001",
        title: $"错误的“{nameof(AttributeTargets)}”",
        messageFormat: $"{{0}}的子类型的“{nameof(AttributeTargets)}”不应在{{1}}之外。",
        category: "设计",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        s_UnexpectedAttributeTargets
    );

    public override void Initialize(AnalysisContext context)
    {
#if false
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeAttributeOperation, OperationKind.Attribute);
    }

    private void AnalyzeAttributeOperation(OperationAnalysisContext context)
    {
        var cancellationToken = context.CancellationToken;

        var aOp = (IAttributeOperation)context.Operation;
        if (context.ContainingSymbol is not INamedTypeSymbol symbol) return;

        if (cancellationToken.IsCancellationRequested) return;

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

        if (cancellationToken.IsCancellationRequested) return;

        var System_AttributeUsageAttribute_TypeSymbol = compilation.GetTypeByMetadataName(typeof(AttributeUsageAttribute).FullName);

        // 检查第一个参数（validOn）是否含有值。
        if (aOp.Operation is not IObjectCreationOperation ocOp) return;
        if (ocOp.Constructor?.ContainingSymbol != System_AttributeUsageAttribute_TypeSymbol) return;
        var args = ocOp.Arguments;
        if (args.Length == 0) return;
        var arg1 = args[0].Value;
        var value1 = arg1.ConstantValue;
        if (!value1.HasValue || value1.Value is null || value1.Value.GetType() != Enum.GetUnderlyingType(typeof(AttributeTargets))) return;

        if (cancellationToken.IsCancellationRequested) return;

        // validOn中的各个标志不能在特定基类规定的应用位置范围之外。
        var validOn = (AttributeTargets)value1.Value;
        var baseAttributeUsageAttributeData = baseAttributeTypeSymbol.GetAttributes().Single(data => data.AttributeClass == System_AttributeUsageAttribute_TypeSymbol);
        var expectedTargets = (AttributeTargets)baseAttributeUsageAttributeData.ConstructorArguments[0].Value!;
        if ((validOn & ~expectedTargets) != 0)
        {
            if (cancellationToken.IsCancellationRequested) return;

            bool elseInherited;
            var initializer = ocOp.Initializer;
            if (initializer is null) elseInherited = false;
            else
            {
                elseInherited = true;
                var baseAttributeUsageAttributeNamedArguments = baseAttributeUsageAttributeData.NamedArguments.ToImmutableDictionary();
                foreach (var assignOp in initializer.Initializers.OfType<ISimpleAssignmentOperation>())
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    if (assignOp.Target is not IPropertyReferenceOperation prop) continue;
                    var constValue = assignOp.Value.ConstantValue;
                    if (!constValue.HasValue) continue;

                    var value = constValue.Value;
                    var propName = prop.Property.Name;
                    var baseValue = baseAttributeUsageAttributeNamedArguments.TryGetValue(propName, out var typedConstant) ? typedConstant.Value :
                        propName switch
                        {
                            nameof(AttributeUsageAttribute.AllowMultiple) => false,
                            nameof(AttributeUsageAttribute.Inherited) => true,
                            _ => null
                        };
                    if (value != baseValue)
                    {
                        elseInherited = false;
                        break;
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested) return;

            context.ReportDiagnostic(Diagnostic.Create(
                s_UnexpectedAttributeTargets,
                location: arg1.Syntax.GetLocation(),
                properties: CreateProperties(expectedTargets, validOn, elseInherited),
                baseAttributeTypeSymbol.Name, Enum.Format(typeof(AttributeTargets), expectedTargets, "F")));
        }
    }

    internal static ImmutableDictionary<string, string?> CreateProperties(
        AttributeTargets expectedTargets,
        AttributeTargets actualTargets,
        bool elseInherited)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string?>();

        builder.Add("Expected", Enum.Format(typeof(AttributeTargets), expectedTargets, "D"));
        builder.Add("Actual", Enum.Format(typeof(AttributeTargets), actualTargets, "D"));
        builder.Add("ElseInherited", elseInherited.ToString());

        return builder.ToImmutable();
    }

    internal static bool TryGetProperties(ImmutableDictionary<string, string?> properties,
        out AttributeTargets expectedTargets,
        out AttributeTargets actualTargets,
        out bool elseInherited)
    {
        var result = false;
        expectedTargets = default;
        actualTargets = default;
        elseInherited = default;

        result &= properties.TryGetValue("Expected", out var propExpectedTargets) && propExpectedTargets is not null &&
            Enum.TryParse(propExpectedTargets, out expectedTargets);
        result &= properties.TryGetValue("Actual", out var propActualTargets) && propActualTargets is not null &&
            Enum.TryParse(propActualTargets, out actualTargets);
        result &= properties.TryGetValue("ElseInherited", out var propElseInherited) && propElseInherited is not null &&
            bool.TryParse(propElseInherited, out elseInherited);

        return result;
    }
}
