// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Qtyi.Runtime;

partial class Table
{
    protected internal struct ListEnumerator : IEnumerator<Object>
    {
        private bool _started = false;
        private Number _index = 0L;
        private readonly Table _underlying;

        public ListEnumerator(Table underlying) => this._underlying = underlying;

        public Object Current
        {
            get
            {
                if (!this._started) throw new InvalidOperationException();

                var value = this._underlying.dictionary[this._index];
                if (value is null) throw new InvalidOperationException();

                return value;
            }
        }

        object? IEnumerator.Current => this.Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            this._started = true;
            this._index++;
            return this._underlying.dictionary[this._index] is not null;
        }

        public void Reset()
        {
            this._started = false;
            this._index = 0L;
        }
    }
}
