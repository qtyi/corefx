// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Text;

namespace Qtyi.Runtime;

partial class String
{
    public static class Encoding
    {
        private static readonly System.Text.Encoding s_Default;

        public static System.Text.Encoding Default => s_Default;

        static Encoding()
        {
            s_Default = (System.Text.Encoding)new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).Clone();
            s_Default.DecoderFallback = new DecoderFallback();
            s_Default.EncoderFallback = new EncoderFallback();
        }

        private sealed class DecoderFallback : System.Text.DecoderFallback
        {
            public override int MaxCharCount => 24;

            public override System.Text.DecoderFallbackBuffer CreateFallbackBuffer() => new DecoderFallbackBuffer();

            public override bool Equals(object? obj) => obj is DecoderFallback;

            public override int GetHashCode() => this.GetType().GetHashCode();
        }

        private sealed class DecoderFallbackBuffer : System.Text.DecoderFallbackBuffer
        {
            private string _replacement = null!;
            private int _length = 0;
            private int _position = 0;

            public override int Remaining => this._length - this._position;

            public override bool Fallback(byte[] bytesUnknown, int index)
            {
                this._replacement = string.Concat(bytesUnknown.Select(static b => $"\\x{b:X2}"));
                this._length = this._replacement.Length;
                this._position = 0;

                return true;
            }

            public override char GetNextChar()
            {
                if (this._position == this._length)
                    return '\0';
                else
                    return this._replacement[this._position++];
            }

            public override bool MovePrevious() => this._length > 0 && this._position > 0;
        }

        private sealed class EncoderFallback : System.Text.EncoderFallback
        {
            public override int MaxCharCount => 2;

            public override System.Text.EncoderFallbackBuffer CreateFallbackBuffer() => new EncoderFallbackBuffer();

            public override bool Equals(object? obj) => obj is EncoderFallback;

            public override int GetHashCode() => this.GetType().GetHashCode();
        }

        private sealed class EncoderFallbackBuffer : System.Text.EncoderFallbackBuffer
        {
            private string _replacement = null!;
            private int _length = 0;
            private int _position = 0;

            public override int Remaining => this._length - this._position;

            public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
            {
                this._replacement = new(new[] { charUnknownHigh, charUnknownLow });
                this._length = this._replacement.Length;
                this._position = 0;

                return true;
            }

            public override bool Fallback(char charUnknown, int index)
            {
                this._replacement = new(new[] { charUnknown });
                this._length = this._replacement.Length;
                this._position = 0;

                return true;
            }

            public override char GetNextChar()
            {
                if (this._position == this._length)
                    return '\0';
                else
                    return this._replacement[this._position++];
            }

            public override bool MovePrevious() => this._length > 0 && this._position > 0;
        }
    }
}
