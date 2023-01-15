// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.CodeAnalysis.CSharp;

internal class CSharpCodeTextWriter : IndentedTextWriter
{
    public const string CSharpTabString = "    ";

    public CSharpCodeTextWriter() : base(CSharpTabString) { }

    public CSharpCodeTextWriter(IFormatProvider? formatProvider) : base(CSharpTabString, formatProvider) { }
}
