// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Diagnostics;
using Qtyi.CodeAnalysis.Testing.XUnit;

namespace Qtyi.CodeAnalysis.VisualBasic.Testing.XUnit;

internal class AnalyzerVerifier<TAnalyzer, TTest> : CommonAnalyzerVerifier<TAnalyzer, TTest>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TTest : AnalyzerTest<TAnalyzer>, new()
{
}

internal class AnalyzerVerifier<TAnalyzer> : CommonAnalyzerVerifier<TAnalyzer, AnalyzerTest<TAnalyzer>>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
}
