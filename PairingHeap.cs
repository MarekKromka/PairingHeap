using System;
using System.Collections;
using System.Collections.Generic;

namespace PairingHeap
{
    public class PairingHeap<T, TKey> : IEnumerable<PairingHeap<T,TKey>.PairingHeapNode> where TKey : IComparable<TKey>, IMinimumKey
    {
        public class PairingHeapNode
        {
            public T Value { get; set; }
            public TKey Priority { get; set; }
            public PairingHeapNode left { get; set; }
            public PairingHeapNode right { get; set; }
            public PairingHeapNode parent { get; set; }

            public PairingHeapNode(T value, TKey priority)
            {
                Value = value;
                Priority = priority;
            }

            public void DeleteConnections()
            {
                left = null;
                right = null;
                parent = null;
            }
        }

        public PairingHeapNode Min { get; set; }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                if (Count == 0) Min = null;
            }
        }

        public PairingHeapNode Insert(T data, TKey priority)
        {
            PairingHeapNode newNode = new PairingHeapNode(data, priority);
            if (Min == null)
            {
                Min = newNode;
            }
            else
            {
                Pair(newNode);
            }

            Count++;
            return newNode;
        }

        public void Insert(PairingHeapNode node)
        {
            if (Min == null)
            {
                Min = node;
            }
            else
            {
                Pair(node);
            }

            Count++;
        }

        private void Pair(PairingHeapNode newNode)
        {
            if (Min.Priority.CompareTo(newNode.Priority) < 0)
            {
                //newNode has greater priority
                newNode.left = Min;
                Min.parent = newNode;
                Min = newNode;
            }
            else
            {
                // new node has lesser prority
                newNode.parent = Min;

                newNode.right = Min.left;
                if(newNode.right != null) newNode.right.parent = newNode;

                Min.left = newNode;
            }
        }

        private PairingHeapNode Pair(PairingHeapNode node1, PairingHeapNode node2)
        {
            // node 1 priority is less -> less means greater number
            if (node1.Priority.CompareTo(node2.Priority) < 0)
            {
                node1.right = node2.left;
                if (node1.right != null) node1.right.parent = node1;

                node2.left = node1;
                node1.parent = node2;
                return node2;
            }
            //node 1 priority is greater
            node2.right = node1.left;
            if (node2.right != null) node2.right.parent = node2;
            node1.left = node2;
            node2.parent = node1;
            return node1;
        }

        public T Peek()
        {
            return Min.Value;
        }

        public T Pop()
        {
            PairingHeapNode returnVal = Min;

            if (Count > 1)
            {
                PairingHeapNode child = Min.left;
                Queue<PairingHeapNode> pairingQueue = new Queue<PairingHeapNode>();
                pairingQueue.Enqueue(child);
                while (child.right != null)
                {
                    PairingHeapNode rightChild = child.right;
                    pairingQueue.Enqueue(child.right);
                    child.parent = null;
                    child.right = null;
                    child = rightChild;
                }

                //set last child parent to null just to make sure to not mess something or we can set Min parent to be null
                child.parent = null;
                while (pairingQueue.Count != 1)
                {
                    pairingQueue.Enqueue(Pair(pairingQueue.Dequeue(), pairingQueue.Dequeue()));
                }

                Min.DeleteConnections();
                Min = pairingQueue.Dequeue();
            }

            Count--;
            return returnVal.Value;
        }

        public void CheckPriority(PairingHeapNode node)
        {
            if(node == null) return;

            //Check if child priority is greater
            CheckChildPriority(ref node);

            // Check if parent priority is greater
            if (node != Min)
            {
                CheckParentPriority(node);
            }
        }

        public bool CheckChildPriority(ref PairingHeapNode node,string p = "")
        {
            if (node.left == null || node.Priority.CompareTo(node.left.Priority) > 0) return false;
            PairingHeapNode changedPriority = node;

            //Mark place where we took node from and remove connections
            PairingHeapNode placeToPut = node.parent;
            bool right = false;
            if (node == node.parent?.right)
            {
                right = true;
                node.parent.right = null;
            }
            else if (node == node.parent?.left)
            {
                node.parent.left = null;
            }

            node.parent = null;

            Queue<PairingHeapNode> pairingQueue = new Queue<PairingHeapNode>();
            
            pairingQueue.Enqueue(changedPriority);
            pairingQueue.Enqueue(changedPriority.left);
            node = changedPriority.left;
            changedPriority.left.parent = null;
            changedPriority.left = null;
            while (node.right != null)
            {
                PairingHeapNode rightChild = node.right;
                pairingQueue.Enqueue(node.right);
                node.parent = null;
                node.right = null;
                node = rightChild;
            }
            node.parent = null;

            if (changedPriority.right != null)
            {
                pairingQueue.Enqueue(changedPriority.right);
                node = changedPriority.right;
                changedPriority.right.parent = null;
                changedPriority.right = null;
                while (node.right != null)
                {
                    PairingHeapNode rightChild = node.right;
                    pairingQueue.Enqueue(node.right);
                    node.parent = null;
                    node.right = null;
                    node = rightChild;
                }
                node.parent = null;
            }

            while (pairingQueue.Count != 1)
            {
                pairingQueue.Enqueue(Pair(pairingQueue.Dequeue(), pairingQueue.Dequeue()));
            }

            node = pairingQueue.Dequeue();
            if (changedPriority == Min) Min = node;
            else if (right) placeToPut.right = node;
            else placeToPut.left = node;
            node.parent = placeToPut;
            return true;
        }

        private void CheckParentPriority(PairingHeapNode node)
        {
            if (node.Priority.CompareTo(node.parent.Priority) < 0) return;

            if (node == node.parent.left) node.parent.left = null;
            else node.parent.right = null;

            Queue<PairingHeapNode> pairingQueue = new Queue<PairingHeapNode>();
            pairingQueue.Enqueue(Min);
            pairingQueue.Enqueue(node);
            while (node.right != null)
            {
                PairingHeapNode rightChild = node.right;
                pairingQueue.Enqueue(node.right);
                node.parent = null;
                node.right = null;
                node = rightChild;
            }

            //set last child parent to null just to make sure to not mess something or we can set Min parent to be null
            node.parent = null;
            while (pairingQueue.Count != 1)
            {
                pairingQueue.Enqueue(Pair(pairingQueue.Dequeue(), pairingQueue.Dequeue()));
            }

            Min = pairingQueue.Dequeue();
        }

        public T DeleteNode(PairingHeapNode node)
        {
            if (node == null) return node.Value;
            node.Priority.setKeyToMin();
            CheckPriority(node);
            return Pop();
        }

        public IEnumerator<PairingHeapNode> GetEnumerator()
        {
            return new PairingHeapEnumerator<T, TKey>(Min);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
