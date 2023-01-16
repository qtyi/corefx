// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Qtyi.CodeAnalysis;

public abstract class IndentedStringWriter : StringWriter
{
    private int _indentLevel = 0;
    private bool _needLeadingIndent = true;
    protected readonly string TabString;

    protected IndentedStringWriter(string tabString, IFormatProvider? formatProvider = null) : base(formatProvider)
    {
        this.TabString = tabString;
    }

    #region Indent
    public void Indent() => this._indentLevel++;
    public void Unindent()
    {
        if (this._indentLevel == 0) return;
        this._indentLevel--;
    }
    public void ResetIndent() => this._indentLevel = 0;
    #endregion

    #region LeadingIndents
    protected void WriteLeadingIndents()
    {
        if (!this._needLeadingIndent) return;

        for (var i = 0; i < this._indentLevel; i++)
        {
            base.Write(this.TabString);
        }
        this._needLeadingIndent = false;
    }
    protected async Task WriteLeadingIndentsAsync()
    {
        await Task.Run(() =>
        {
            if (!this._needLeadingIndent) return;

            for (var i = 0; i < this._indentLevel; i++)
            {
                base.Write(this.TabString);
            }
            this._needLeadingIndent = false;
        }).ConfigureAwait(false);
    }

    protected void NeedLeadingIndents() => this._needLeadingIndent = true;
    #endregion

    #region Write
    public override void Write(bool value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(char value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(char[]? buffer)
    {
        this.WriteLeadingIndents();
        base.Write(buffer);
    }
    public override void Write(char[] buffer, int index, int count)
    {
        this.WriteLeadingIndents();
        base.Write(buffer, index, count);
    }
    public override void Write(decimal value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(double value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(int value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(long value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(object? value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
#if NETCOREAPP
    public override void Write(ReadOnlySpan<char> buffer)
    {
        this.WriteLeadingIndents();
        base.Write(buffer);
    }
#endif
    public override void Write(float value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string? value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0)
    {
        this.WriteLeadingIndents();
        base.Write(format, arg0);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1)
    {
        this.WriteLeadingIndents();
        base.Write(format, arg0, arg1);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1, object? arg2)
    {
        this.WriteLeadingIndents();
        base.Write(format, arg0, arg1, arg2);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, params object?[] arg)
    {
        this.WriteLeadingIndents();
        base.Write(format, arg);
    }
#if NETCOREAPP
    public override void Write(StringBuilder? value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
#endif
    public override void Write(uint value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override void Write(ulong value)
    {
        this.WriteLeadingIndents();
        base.Write(value);
    }
    public override async Task WriteAsync(char value)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteAsync(value).ConfigureAwait(false);
    }
    public override async Task WriteAsync(char[] buffer, int index, int count)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteAsync(buffer, index, count).ConfigureAwait(false);
    }
#if NETCOREAPP
    public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
    }
#endif
    public override async Task WriteAsync(string? value)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteAsync(value).ConfigureAwait(false);
    }
#if NETCOREAPP
    public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteAsync(value, cancellationToken).ConfigureAwait(false);
    }
#endif
#endregion

    #region WriteLine
    public override void WriteLine()
    {
        this.WriteLeadingIndents();
        base.WriteLine();
        this.NeedLeadingIndents();
    }
    public override void WriteLine(bool value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char[]? buffer)
    {
        this.WriteLeadingIndents();
        base.WriteLine(buffer);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char[] buffer, int index, int count)
    {
        this.WriteLeadingIndents();
        base.WriteLine(buffer, index, count);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(decimal value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(double value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(int value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(long value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(object? value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        this.WriteLeadingIndents();
        base.WriteLine(buffer);
        this.NeedLeadingIndents();
    }
#endif
    public override void WriteLine(float value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(string? value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0)
    {
        this.WriteLeadingIndents();
        base.WriteLine(format, arg0);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1)
    {
        this.WriteLeadingIndents();
        base.WriteLine(format, arg0, arg1);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1, object? arg2)
    {
        this.WriteLeadingIndents();
        base.WriteLine(format, arg0, arg1, arg2);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, params object?[] arg)
    {
        this.WriteLeadingIndents();
        base.WriteLine(format, arg);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override void WriteLine(StringBuilder? value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
#endif
    public override void WriteLine(uint value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(ulong value)
    {
        this.WriteLeadingIndents();
        base.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync()
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync().ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync(char value)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync(value).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync(char[] buffer, int index, int count)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync(buffer, index, count).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync(buffer, cancellationToken).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#endif
    public override async Task WriteLineAsync(string? value)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync(value).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        await this.WriteLeadingIndentsAsync().ConfigureAwait(false);
        await base.WriteLineAsync(value, cancellationToken).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#endif
    #endregion
}
