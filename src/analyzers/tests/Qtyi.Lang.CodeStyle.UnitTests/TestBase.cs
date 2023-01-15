// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CodeAnalysis.Testing;

namespace Qtyi.CodeAnalysis.UnitTests;

public abstract class TestBase
{
    protected static string BuildSource<TWriter>(Action<TWriter> writeAction)
        where TWriter : IndentedTextWriter, new()
    {
        var writer = new TWriter();
        writeAction(writer);
        return writer.ToString();
    }
}
