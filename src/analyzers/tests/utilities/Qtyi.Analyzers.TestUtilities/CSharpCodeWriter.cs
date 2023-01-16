// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.CodeAnalysis.CSharp;

internal class CSharpCodeWriter : IndentedStringWriter
{
    public const string CSharpTabString = "    ";

    public CSharpCodeWriter() : base(CSharpTabString) { }

    public CSharpCodeWriter(IFormatProvider? formatProvider) : base(CSharpTabString, formatProvider) { }
}
