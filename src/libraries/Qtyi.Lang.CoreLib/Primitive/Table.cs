// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Qtyi.Runtime;

using static Metatable;

public partial class Table : Object, IList<Object>, IDictionary<Object, Object>
{
    protected internal Table? mt;
    protected internal readonly Dictionary<Object, Object> dictionary = new();

    public override Object? this[Object? key]
    {
        get
        {
            // 首先在字典中查找键对应的值。
            Object? value;
            if (key is null) // 字典不支持空值。
                value = null;
            else
                value = RawGet(this, key);

            // 若字典中不存在键对应的值，再通过元表查找键对应的值。
            value ??= this.GetMetatableIndex(key);

            return value;
        }
        set
        {
            if (key is null) throw new ArgumentNullException(nameof(value)); // 字典不支持空值。

            // 若字典中存在指定键；或虽不存在指定键，但通过元表设置指定键的值失败。
            if (this.dictionary.ContainsKey(key) || !this.SetMetatableIndex(key, value))
                // 直接设置字典中键对应的值。
                RawSet(this, key, value);
        }
    }

    /// <summary>
    /// 获取表中第<paramref name="offset"/>项开始的序列的项目数量。
    /// </summary>
    protected long GetCount(long offset = 1L)
    {
        Debug.Assert(offset >= 1);
        for (var i = 0L; ; i++)
        {
            if (!this.dictionary.ContainsKey(i + offset))
                return i - 0L;
        }
    }

    /// <summary>
    /// 获取表中第一个序列的长度。
    /// </summary>
    /// <remarks>
    /// 优先返回元表值<see cref="Metavalue_LengthOperation"/>的返回值。
    /// </remarks>
    public sealed override Object? Length
    {
        get
        {
            var mvLength = this.GetMetavalue(Metavalue_LengthOperation);
            if (mvLength is null)
                return this.GetCount();
            else
                return mvLength.Invoke(this)[0];
        }
    }

    protected internal sealed override Table? Metatable
    {
        get => this.mt;
        set => this.mt = value;
    }

    /// <summary>
    /// 设置此实例的元表。
    /// </summary>
    /// <param name="table">一个 Lua 表，作为元表设置到此实例。</param>
    /// <returns>设置成功后的此实例的元表。</returns>
    public virtual Table? SetMetatable(Table? table)
    {
        var metatable = this.Metatable;
        if (metatable is not null)
        {
            var mvMetatable = RawGet(metatable, Metavalue_Metatable);
            if (mvMetatable is not null)
                throw new MetavalueNotFoundException(Metavalue_Metatable);
        }

        this.mt = table;
        return this.mt;
    }

    /// <summary>
    /// 向表中添加一个元素。
    /// </summary>
    /// <param name="value">要添加的元素。</param>
    public void Add(Object? value) => this.Add(this.GetCount() + 1L, value);

    /// <summary>
    /// 向表中添加多个元素。
    /// </summary>
    /// <param name="values">要添加的元素序列。</param>
    public void AddRange(IEnumerable<Object?> values)
    {
        var index = this.GetCount() + 1L;
        foreach (var value in values)
        {
            this.Add(index++, value);
            while (this[index] is not null) index++;
        }
    }

    /// <summary>
    /// 向表中添加指定的索引和元素。
    /// </summary>
    /// <param name="index">要添加到的索引。</param>
    /// <param name="value">要添加的元素。</param>
    public virtual void Add(Object index, Object? value) => this[index] = value;

    /// <summary>
    /// 向表中指定索引位置所在的序列中插入一个元素。
    /// </summary>
    /// <param name="index">要插入的索引位置（以<c>1</c>为基数）。</param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual void Insert(long index, Object? value)
    {
        if (index < 1L) throw new ArgumentOutOfRangeException(nameof(index));

        var count = this.GetCount(index);

        if (count != 0)
        {
            this.dictionary.Add(index + count, this.dictionary[index + count - 1]);
            for (var i = index + count - 2; i >= index; i--)
            {
                this.dictionary[i + 1] = this.dictionary[i];
            }
        }

        if (value is null)
            this.dictionary.Remove(index);
        else
        {
            this.dictionary[index] = value;
        }
    }

    /// <summary>
    /// 移除表中指定索引位置的元素。
    /// </summary>
    /// <param name="index">要移除的索引位置（以<c>1</c>为基数）。</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual Object? RemoveAt(long index)
    {
        if (index < 1) throw new ArgumentOutOfRangeException(nameof(index));

        var count = this.GetCount(index);
        if (count == 0) return null;

        long max = index + count;
        if (this.Length is Number length)
        {
            if (index > length) throw new ArgumentOutOfRangeException(nameof(index));

            max = Math.Min(length.ChangeType<long>(), max);
        }
        else throw new InvalidLengthOperationException("object length is not an integer");

        var removed = this[index];

        for (var i = index; i < max; i++)
        {
            this.dictionary[i] = this.dictionary[i + 1];
        }
        this.dictionary.Remove(max);

        return removed;
    }

    /// <summary>
    /// 获取表中指定键对应的值，忽略元表的影响。
    /// </summary>
    /// <param name="table">值所在的表。</param>
    /// <param name="index">要查询的键。</param>
    /// <returns></returns>
    public static Object? RawGet(Table table, Object index)
    {
        if (table is null) throw new ArgumentNullException(nameof(table));
        if (index is null) throw new ArgumentNullException(nameof(index));

        if (table.dictionary.TryGetValue(index, out var value))
            return value;
        else
            return null;
    }

    /// <summary>
    /// 获取表中指定键对应的值，忽略元表的影响。
    /// </summary>
    /// <param name="table"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Table RawSet(Table table, Object index, Object? value)
    {
        if (table is null) throw new ArgumentNullException(nameof(table));
        if (index is null) throw new ArgumentNullException(nameof(index));

        if (table.dictionary.ContainsKey(index))
        {
            if (value is null)
                table.dictionary.Remove(index);
            else
                table.dictionary[index] = value;
        }
        else if (value is not null)
            table.dictionary.Add(index, value);

        return table;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        if (this.mt is not null)
            hashCode = HashCode.Combine(hashCode, this.mt.GetHashCode());
        hashCode = HashCode.Combine(hashCode, this.dictionary.GetHashCode());
        return hashCode;
    }

    public override TypeInfo GetTypeInfo() => TypeInfo.Table;

    protected override String ToStringCore() => $"table: {string.Concat(BitConverter.GetBytes(this.GetHashCode()).Select(b => Convert.ToString(b, 16)))}";

    /// <inheritdoc/>
    /// <exception cref="InvalidCastException"><paramref name="type"/> 不是能接受的转换目标类型。</exception>
    public override object ChangeType(Type type)
    {
        if (typeof(Object).IsAssignableFrom(type) && type.IsAssignableFrom(this.GetType())) return this;
        else throw new InvalidCastException();
    }

    #region IList<Object>
    int ICollection<Object>.Count => checked((int)this.GetCount());

    bool ICollection<Object>.IsReadOnly => false;

    Object IList<Object>.this[int index]
    {
        get => this.dictionary[index + 1L];
        set => this.dictionary[index + 1L] = value;
    }

    bool ICollection<Object>.Contains(Object item)
    {
        var index = ((IList<Object>)this).IndexOf(item);
        return index >= 0;
    }

    void ICollection<Object>.CopyTo(Object[] array, int arrayIndex)
    {
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        for (long i = 0L, length = array.Length - arrayIndex; i < length; i++)
        {
            if (!this.dictionary.TryGetValue(i + 1L, out var value)) break;
            array[arrayIndex + i] = value;
        }
    }

    void ICollection<Object>.Clear()
    {
        var index = 1L;
        while (this.dictionary.Remove(index)) index++;
    }

    IEnumerator<Object> IEnumerable<Object>.GetEnumerator() => new ListEnumerator(this);

    int IList<Object>.IndexOf(Object item)
    {
        for (int i = 1; ; i++)
        {
            if (object.Equals(this.dictionary[i], item))
                return i - 1;
        }
    }

    void IList<Object>.Insert(int index, Object item) => this.Insert(index + 1, item);

    bool ICollection<Object>.Remove(Object item)
    {
        var index = ((IList<Object>)this).IndexOf(item);
        if (index < 0) return false;

        ((IList<Object>)this).RemoveAt(index);
        return true;
    }

    void IList<Object>.RemoveAt(int index) => this.RemoveAt(index + 1);
    #endregion

    #region IDictionary<Object, Object>
    Object IDictionary<Object, Object>.this[Object key]
    {
        get => this.dictionary[key];
        set => this.dictionary[key] = value;
    }

    int ICollection<KeyValuePair<Object, Object>>.Count => this.dictionary.Count;

    bool ICollection<KeyValuePair<Object, Object>>.IsReadOnly => false;

    ICollection<Object> IDictionary<Object, Object>.Keys => this.dictionary.Keys;

    ICollection<Object> IDictionary<Object, Object>.Values => this.dictionary.Values;

    void IDictionary<Object, Object>.Add(Object key, Object value) => RawSet(this, key, value);

    void ICollection<KeyValuePair<Object, Object>>.Add(KeyValuePair<Object, Object> item) => RawSet(this, item.Key, item.Value);

    void ICollection<KeyValuePair<Object, Object>>.Clear() => this.dictionary.Clear();

    bool ICollection<KeyValuePair<Object, Object>>.Contains(KeyValuePair<Object, Object> item) => object.Equals(RawGet(this, item.Key), item.Value);

    void ICollection<KeyValuePair<Object, Object>>.CopyTo(KeyValuePair<Object, Object>[] array, int arrayIndex) => ((ICollection<KeyValuePair<Object, Object>>)this).CopyTo(array, arrayIndex);

    bool IDictionary<Object, Object>.ContainsKey(Object key) => this.dictionary.ContainsKey(key);

    IEnumerator<KeyValuePair<Object, Object>> IEnumerable<KeyValuePair<Object, Object>>.GetEnumerator() => this.dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.dictionary.GetEnumerator();

    bool IDictionary<Object, Object>.Remove(Object key) => this.dictionary.Remove(key);

    bool ICollection<KeyValuePair<Object, Object>>.Remove(KeyValuePair<Object, Object> item) => ((ICollection<KeyValuePair<Object, Object>>)this).Remove(item);

    bool IDictionary<Object, Object>.TryGetValue(Object key, [MaybeNullWhen(false)] out Object value) => this.dictionary.TryGetValue(key, out value);
    #endregion

    public static MultiReturns<Object, Object> Next(Table table, Object? index = null)
    {
        if (table is null) throw new ArgumentNullException(nameof(table));

        IEnumerator<KeyValuePair<Object, Object>> etor = table.dictionary.GetEnumerator();
        if (index is not null)
        {
            while (etor.MoveNext())
            {
                if (table.dictionary.Comparer.Equals(etor.Current.Key, index))
                    break;
            }
        }

        if (etor.MoveNext())
        {
            var pair = etor.Current;
            return new(pair.Key, pair.Value);
        }
        else
            return MultiReturns<Object, Object>.Empty;
    }
}
