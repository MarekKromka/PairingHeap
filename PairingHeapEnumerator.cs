using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PairingHeap
{
    class PairingHeapEnumerator<T,TKey> : IEnumerator<PairingHeap<T, TKey>.PairingHeapNode> where TKey : IComparable<TKey>, IMinimumKey
    {
        private Queue<PairingHeap<T, TKey>.PairingHeapNode> enumData = new Queue<PairingHeap<T, TKey>.PairingHeapNode>();

        public PairingHeapEnumerator(PairingHeap<T, TKey>.PairingHeapNode minimum)
        {
            Current = minimum;
            if(Current != null) enumData.Enqueue(Current);
        }

        public PairingHeap<T, TKey>.PairingHeapNode Current { get; private set; }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            enumData = null;
        }

        public bool MoveNext()
        {
            if (enumData.Count <= 0) return false;

            Current = enumData.Dequeue();
            if (Current.left != null) enumData.Enqueue(Current.left);
            if (Current.right != null) enumData.Enqueue(Current.right);
            return true;

        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
