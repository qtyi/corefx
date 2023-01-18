// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace Qtyi.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class MultiReturnsRestGenericArgumentDiagnosticAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor UnsupportedTRestType = new(
        "QTYI1002",
        title: $"不受支持的“Qtyi.Runtime.MultiReturns`8”的泛型参数“TRest”",
        messageFormat: $"{{0}}不受支持的“Qtyi.Runtime.MultiReturns`8”的泛型参数“TRest”，“TRest”必须是泛型的“MultiReturns”类型。",
        category: "设计",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        UnsupportedTRestType
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
        context.RegisterSyntaxNodeAction(AnalyzeNode, (CS.SyntaxKind[])Enum.GetValues(typeof(CS.SyntaxKind)));
        context.RegisterSyntaxNodeAction(AnalyzeNode, (VB.SyntaxKind[])Enum.GetValues(typeof(VB.SyntaxKind)));
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var model = context.SemanticModel;
        var supportedMultiReturnsTypeSymbols = new Lazy<ImmutableArray<INamedTypeSymbol?>>(() => ImmutableArray.Create(
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`1"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`2"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`3"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`4"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`5"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`6"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`7"),
            model.Compilation.GetTypeByMetadataName("Qtyi.Runtime.MultiReturns`8")
        ));
        switch (context.Node)
        {
            case CS.Syntax.GenericNameSyntax type:
                {
                    var symbol = model.GetSymbolInfo(type, context.CancellationToken).Symbol as INamedTypeSymbol;
                    if (symbol is not null && IsMultiReturns8(symbol) &&
                        symbol.TypeArguments[7] is INamedTypeSymbol restTypeSymbol && restTypeSymbol.TypeKind != TypeKind.Error && !IsSupportedMultiReturnsTypes(restTypeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            UnsupportedTRestType,
                            location: type.TypeArgumentList.Arguments[7].GetLocation(),
                            $"类型“{restTypeSymbol.Name}”是"));
                    }
                }
                break;
            case VB.Syntax.GenericNameSyntax type:
                {
                    var symbol = model.GetSymbolInfo(type, context.CancellationToken).Symbol as INamedTypeSymbol;
                    if (symbol is not null && IsMultiReturns8(symbol) &&
                        symbol.TypeArguments[7] is INamedTypeSymbol restTypeSymbol && restTypeSymbol.TypeKind != TypeKind.Error && !IsSupportedMultiReturnsTypes(restTypeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            UnsupportedTRestType,
                            location: type.TypeArgumentList.Arguments[7].GetLocation(),
                            $"类型“{restTypeSymbol.Name}”是"));
                    }
                }
                break;

            case CS.Syntax.ExpressionSyntax expression:
                {
                    if (expression is CS.Syntax.TypeSyntax) break;

                    var symbol = model.GetTypeInfo(expression, context.CancellationToken).Type as INamedTypeSymbol;
                    if (symbol is not null && IsMultiReturns8(symbol) &&
                        symbol.TypeArguments[7] is INamedTypeSymbol restTypeSymbol && restTypeSymbol.TypeKind != TypeKind.Error && !IsSupportedMultiReturnsTypes(restTypeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            UnsupportedTRestType,
                            location: expression.GetLocation(),
                            $"表达式类型包含"));
                    }
                }
                break;
            case VB.Syntax.ExpressionSyntax expression:
                {
                    if (expression is VB.Syntax.TypeSyntax) break;

                    var symbol = model.GetTypeInfo(expression, context.CancellationToken).Type as INamedTypeSymbol;
                    if (symbol is not null && IsMultiReturns8(symbol) &&
                        symbol.TypeArguments[7] is INamedTypeSymbol restTypeSymbol && restTypeSymbol.TypeKind != TypeKind.Error && !IsSupportedMultiReturnsTypes(restTypeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            UnsupportedTRestType,
                            location: expression.GetLocation(),
                            $"表达式类型包含"));
                    }
                }
                break;
        }

        bool IsMultiReturns8(INamedTypeSymbol symbol) => symbol.ConstructedFrom == supportedMultiReturnsTypeSymbols.Value[7];

        bool IsSupportedMultiReturnsTypes(INamedTypeSymbol symbol) => supportedMultiReturnsTypeSymbols.Value.Contains(symbol.ConstructedFrom);
    }
}
