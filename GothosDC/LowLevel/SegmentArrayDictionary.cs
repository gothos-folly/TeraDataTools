using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GothosDC.LowLevel
{
    internal class VirtualReadOnlyCollection<T> : ICollection<T>
    {
        private readonly IEnumerable<T> _sequence;
        private readonly int _count;

        public VirtualReadOnlyCollection(IEnumerable<T> sequence, int count)
        {
            _sequence = sequence;
            _count = count;
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _sequence.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in _sequence)
            {
                array[arrayIndex++] = item;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class SegmentAddressDictionary<TValue> : IDictionary<SegmentAddress, TValue>
    {
        private readonly TValue[] _data;
        private readonly BitArray _used;

        public int Capacity { get { return _data.Length; } }
        public int Count { get; private set; }

        public SegmentAddressDictionary(IEnumerable<KeyValuePair<SegmentAddress, TValue>> sequence, SegmentAddress max)
        {
            _data = new TValue[max.ToInt() + 1];
            _used = new BitArray(_data.Length, false);
            foreach (var pair in sequence)
            {
                var index = pair.Key.ToInt();
                _data[index] = pair.Value;
                _used[index] = true;
                Count++;
            }
            Keys = new VirtualReadOnlyCollection<SegmentAddress>(this.Select(x => x.Key), Count);
            Values = new VirtualReadOnlyCollection<TValue>(this.Select(x => x.Value), Count);
        }

        bool ICollection<KeyValuePair<SegmentAddress, TValue>>.Contains(KeyValuePair<SegmentAddress, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value))
                return false;
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<SegmentAddress, TValue>[] array, int arrayIndex)
        {
            var data = _data;
            for (int i = 0; i <= data.Length; i++)
            {
                if (!_used[i])
                    continue;
                array[i + arrayIndex] = new KeyValuePair<SegmentAddress, TValue>(new SegmentAddress((uint)i), data[i]);
            }
        }

        public IEnumerator<KeyValuePair<SegmentAddress, TValue>> GetEnumerator()
        {
            var data = _data;
            for (int i = 0; i < data.Length; i++)
            {
                if (!_used[i])
                    continue;
                yield return new KeyValuePair<SegmentAddress, TValue>(new SegmentAddress((uint)i), data[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(SegmentAddress key)
        {
            var index = key.ToInt();
            if ((uint)index >= _used.Length)
                return false;
            return _used[index];
        }

        public bool TryGetValue(SegmentAddress key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }
            value = _data[key.ToInt()];
            return true;
        }

        public TValue this[SegmentAddress key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();
                return value;
            }
            set { throw new NotSupportedException(); }
        }

        public ICollection<SegmentAddress> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }

        public void Add(SegmentAddress key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool Remove(SegmentAddress key)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<SegmentAddress, TValue>>.Add(KeyValuePair<SegmentAddress, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<SegmentAddress, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<SegmentAddress, TValue>>.Remove(KeyValuePair<SegmentAddress, TValue> item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<SegmentAddress, TValue>>.IsReadOnly
        {
            get { return true; }
        }
    }
}
