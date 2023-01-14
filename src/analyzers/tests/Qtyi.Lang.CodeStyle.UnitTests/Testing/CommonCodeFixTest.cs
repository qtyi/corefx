// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
namespace Qtyi.CodeAnalysis.Testing.XUnit;

internal abstract class CommonCodeFixTest : CodeFixTest<XUnitVerifier>
{
    protected CommonCodeFixTest()
    {
        this.ReferenceAssemblies = CommonAnalyzerTest.FrameworkReferenceAssemblies;
        this.TestState.AdditionalReferences.Add(CommonAnalyzerTest.Assembly_QtyiLangCoreLib);
    }
}
