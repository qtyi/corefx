// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Qtyi.CodeAnalysis.Testing.XUnit;

internal abstract class CommonAnalyzerTest : AnalyzerTest<XUnitVerifier>
{
    protected static readonly ReferenceAssemblies FrameworkReferenceAssemblies =
#if NET472
        ReferenceAssemblies.NetStandard.NetStandard20;
#elif NETCOREAPP3_1
        ReferenceAssemblies.NetCore.NetCoreApp31;
#elif NET7_0
        ReferenceAssemblies.Net.Net60;
#endif

    protected static readonly Assembly Assembly_QtyiLangCoreLib = typeof(Qtyi.Runtime.Object).Assembly;
    protected static readonly string AssemblyLocation_QtyiLangCoreLib = Assembly_QtyiLangCoreLib.Location;

    protected CommonAnalyzerTest()
    {
        this.ReferenceAssemblies = FrameworkReferenceAssemblies;
        this.TestState.AdditionalReferences.Add(Assembly_QtyiLangCoreLib);
    }
}
