// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Qtyi.CodeAnalysis.Testing.XUnit;

internal abstract class CommonCodeFixVerifier<TAnalyzer, TCodeFix, TTest> : CodeFixVerifier<TAnalyzer, TCodeFix, TTest, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
    where TTest : CommonCodeFixTest, new()
{
}
