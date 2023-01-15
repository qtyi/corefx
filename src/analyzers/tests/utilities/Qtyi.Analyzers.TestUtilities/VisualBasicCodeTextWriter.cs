// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qtyi.CodeAnalysis.VisualBasic;

internal class VisualBasicCodeTextWriter : IndentedTextWriter
{
    public const string VisualBasicTabString = "    ";

    public VisualBasicCodeTextWriter() : base(VisualBasicTabString) { }

    public VisualBasicCodeTextWriter(IFormatProvider? formatProvider) : base(VisualBasicTabString, formatProvider) { }
}
