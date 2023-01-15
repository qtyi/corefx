// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Qtyi.CodeAnalysis.Testing.XUnit;

namespace Qtyi.CodeAnalysis.CSharp.Testing.XUnit;

internal class AnalyzerTest<TAnalyzer> : CommonAnalyzerTest
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    private static readonly LanguageVersion DefaultLanguageVersion =
        Enum.TryParse("Default", out LanguageVersion version) ? version : LanguageVersion.CSharp6;

    protected override string DefaultFileExt => "cs";

    public override string Language => LanguageNames.CSharp;

    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(DefaultLanguageVersion, DocumentationMode.Diagnose);

    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        => new[] { new TAnalyzer() };
}
