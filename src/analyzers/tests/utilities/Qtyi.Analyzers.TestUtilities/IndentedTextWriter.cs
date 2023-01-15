// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Qtyi.CodeAnalysis;

public abstract class IndentedTextWriter : TextWriter
{
    private int _indentLevel = 0;
    private bool _needLeadingIndent = false;
    protected readonly string TabString;

    public override Encoding Encoding => this.InnerWriter.Encoding;
    public override IFormatProvider FormatProvider => this.InnerWriter.FormatProvider;
    protected StringWriter InnerWriter { get; init; }
#if NETCOREAPP
    [AllowNull]
#endif
    public override string NewLine { get => this.InnerWriter.NewLine; set => this.InnerWriter.NewLine = value; }

    protected IndentedTextWriter(string tabString, IFormatProvider? formatProvider = null)
    {
        this.InnerWriter = new(formatProvider);
        this.TabString = tabString;
    }

    public void Indent() => this._indentLevel++;
    public void Unindent()
    {
        if (this._indentLevel == 0) return;
        this._indentLevel--;
    }
    public void ResetIndent() => this._indentLevel = 0;

    public override string ToString() => this.InnerWriter.ToString();

    public override void Close() => this.InnerWriter.Close();

    protected override void Dispose(bool disposing)
    {
        if (disposing == false) return;

        this.InnerWriter.Dispose();
        GC.SuppressFinalize(this);
    }

#if NETCOREAPP
    public override ValueTask DisposeAsync() => this.InnerWriter.DisposeAsync();
#endif

    public override void Flush() => this.InnerWriter.Flush();
    public override Task FlushAsync() => this.InnerWriter.FlushAsync();

    protected void WriteLeadingIndent()
    {
        if (!this._needLeadingIndent) return;

        for (var i = 0; i < this._indentLevel; i++)
        {
            this.InnerWriter.Write(this.TabString);
        }
        this._needLeadingIndent = false;
    }
    protected async Task WriteLeadingIndentAsync()
    {
        await Task.Run(() =>
        {
            if (!this._needLeadingIndent) return;

            for (var i = 0; i < this._indentLevel; i++)
            {
                this.InnerWriter.Write(this.TabString);
            }
            this._needLeadingIndent = false;
        }).ConfigureAwait(false);
    }

    protected void NeedLeadingIndents() => this._needLeadingIndent = true;

    #region Write
    public override void Write(bool value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(char value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(char[]? buffer)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(buffer);
    }
    public override void Write(char[] buffer, int index, int count)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(buffer, index, count);
    }
    public override void Write(decimal value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(double value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(int value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(long value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(object? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
#if NETCOREAPP
    public override void Write(ReadOnlySpan<char> buffer)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(buffer);
    }
#endif
    public override void Write(float value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(format, arg0);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(format, arg0, arg1);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1, object? arg2)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(format, arg0, arg1, arg2);
    }
    public override void Write(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, params object?[] arg)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(format, arg);
    }
#if NETCOREAPP
    public override void Write(StringBuilder? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
#endif
    public override void Write(uint value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override void Write(ulong value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.Write(value);
    }
    public override async Task WriteAsync(char value)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteAsync(value).ConfigureAwait(false);
    }
    public override async Task WriteAsync(char[] buffer, int index, int count)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteAsync(buffer, index, count).ConfigureAwait(false);
    }
#if NETCOREAPP
    public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentAsync().ConfigureAwait(false);
        await InnerWriter.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
    }
#endif
    public override async Task WriteAsync(string? value)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteAsync(value).ConfigureAwait(false);
    }
#if NETCOREAPP
    public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentAsync().ConfigureAwait(false);
        await InnerWriter.WriteAsync(value, cancellationToken).ConfigureAwait(false);
    }
#endif
#endregion

    #region WriteLine
    public override void WriteLine()
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine();
        this.NeedLeadingIndents();
    }
    public override void WriteLine(bool value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char[]? buffer)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(buffer);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(char[] buffer, int index, int count)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(buffer, index, count);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(decimal value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(double value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(int value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(long value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(object? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(buffer);
        this.NeedLeadingIndents();
    }
#endif
    public override void WriteLine(float value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(string? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(format, arg0);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(format, arg0, arg1);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, object? arg0, object? arg1, object? arg2)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(format, arg0, arg1, arg2);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(
#if NET7_0_OR_GREATER
        [StringSyntax("CompositeFormat")]
#endif
        string format, params object?[] arg)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(format, arg);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override void WriteLine(StringBuilder? value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
#endif
    public override void WriteLine(uint value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override void WriteLine(ulong value)
    {
        this.WriteLeadingIndent();
        this.InnerWriter.WriteLine(value);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync()
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteLineAsync().ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync(char value)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteLineAsync(value).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
    public override async Task WriteLineAsync(char[] buffer, int index, int count)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteLineAsync(buffer, index, count).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        await WriteLeadingIndentAsync().ConfigureAwait(false);
        await InnerWriter.WriteLineAsync(buffer, cancellationToken).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#endif
    public override async Task WriteLineAsync(string? value)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteLineAsync(value).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#if NETCOREAPP
    public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        await this.WriteLeadingIndentAsync().ConfigureAwait(false);
        await this.InnerWriter.WriteLineAsync(value, cancellationToken).ConfigureAwait(false);
        this.NeedLeadingIndents();
    }
#endif
    #endregion
}
