// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.Diagnostics;
using Qtyi.CodeAnalysis.Testing.XUnit;

namespace Qtyi.CodeAnalysis.VisualBasic.Testing.XUnit;

internal class CodeFixTest<TAnalyzer, TCodeFix> : CommonCodeFixTest
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    private static readonly LanguageVersion DefaultLanguageVersion =
        Enum.TryParse("Default", out LanguageVersion version) ? version : LanguageVersion.VisualBasic14;

    protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        => new[] { new TCodeFix() };

    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        => new[] { new TAnalyzer() };

    protected override string DefaultFileExt => "cs";

    public override string Language => LanguageNames.VisualBasic;

    public override Type SyntaxKindType => typeof(SyntaxKind);

    protected override CompilationOptions CreateCompilationOptions()
        => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

    protected override ParseOptions CreateParseOptions()
        => new VisualBasicParseOptions(DefaultLanguageVersion, DocumentationMode.Diagnose);
}
