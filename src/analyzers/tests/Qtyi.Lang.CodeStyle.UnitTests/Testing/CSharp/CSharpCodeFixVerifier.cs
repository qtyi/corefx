// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Qtyi.CodeAnalysis.Testing.XUnit;

namespace Qtyi.CodeAnalysis.CSharp.Testing.XUnit;

internal class CodeFixVerifier<TAnalyzer, TCodeFix, TTest> : CommonCodeFixVerifier<TAnalyzer, TCodeFix, TTest>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
    where TTest : CodeFixTest<TAnalyzer, TCodeFix>, new()
{
}

internal class CodeFixVerifier<TAnalyzer, TCodeFix> : CommonCodeFixVerifier<TAnalyzer, TCodeFix, CodeFixTest<TAnalyzer, TCodeFix>>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
}
