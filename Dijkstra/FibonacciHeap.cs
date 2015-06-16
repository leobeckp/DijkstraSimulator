using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dijkstra
{
    //Source: https://github.com/sqeezy/FibonacciHeap

    public class FibonacciHeapNode<T>
    {
        private readonly T _element;

        public int Key;

        private FibonacciHeapNode<T> _child;

        private int _degree;

        private FibonacciHeapNode<T> _left;

        private bool _marked;

        private FibonacciHeapNode<T> _parent;

        private FibonacciHeapNode<T> _right;

        public FibonacciHeapNode(T element, int key)
        {
            _degree = 0;
            _left = this;
            _right = this;
            _marked = false;
            this._element = element;
            Key = key;
        }

        public int Degree
        {
            get { return _degree; }
        }

        public T Element
        {
            get { return _element; }
        }

        public bool Marked
        {
            get { return _marked; }
            set { _marked = value; }
        }

        internal FibonacciHeapNode<T> Child
        {
            get { return _child; }
            set { _child = value; }
        }

        internal FibonacciHeapNode<T> Left
        {
            get { return _left; }
            set { _left = value; }
        }

        internal FibonacciHeapNode<T> Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        internal FibonacciHeapNode<T> Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public void DecreaseDegree()
        {
            _degree--;
        }

        public void IncreaseDegree()
        {
            _degree++;
        }
    }

    public class FibonacciHeap<T>
    {
        private static readonly double oneOverLogPhi = 1.0/Math.Log((1.0 + Math.Sqrt(5.0))/2.0);
        private HashSet<T> _allElements = new HashSet<T>();
        private Comparer<T> _comparer;
        private int _markedNodes = 0;
        private FibonacciHeapNode<T> _minimumNode;
        private int _size = 0;
        private int _trees = 0;

        public FibonacciHeap()
            : this(null)
        {
        }

        public FibonacciHeap(Comparer<T> comparer)
        {
            _comparer = comparer;
        }

        public bool Add(T element, int key)
        {
            if (element == null)
            {
                throw new ArgumentException("Elememts to add are not allowed to be null.");
            }

            FibonacciHeapNode<T> node = new FibonacciHeapNode<T>(element, key);

            MoveToRoot(node);

            _size++;

            _allElements.Add(element);

            return true;
        }

        public void Clear()
        {
            _minimumNode = null;
            _size = 0;
            _trees = 0;
            _markedNodes = 0;
            _allElements.Clear();
        }

        public void Consolidate()
        {
            //if the heap is empty now, the procedure is finished
            if (_minimumNode == null)
            {
                return;
            }

            //save info of top level
            List<FibonacciHeapNode<T>> treeTable = new List<FibonacciHeapNode<T>>();

            //cache parts of the heap we need to visit again
            List<FibonacciHeapNode<T>> toVisit = new List<FibonacciHeapNode<T>>();

            //fill toVisit list
            for (FibonacciHeapNode<T> current = _minimumNode;
                toVisit.Count == 0 || toVisit[0] != current;
                current = current.Right)
            {
                toVisit.Add(current);
            }

            //combine pairwise
            foreach (FibonacciHeapNode<T> current in toVisit)
            {
                FibonacciHeapNode<T> tocompare = current;
                while (true)
                {
                    //keep treeTable at a proper size
                    while (current.Degree >= treeTable.Count)
                    {
                        treeTable.Add(null);
                    }

                    if (treeTable[current.Degree] == null)
                    {
                        treeTable[current.Degree] = current;
                        break;
                    }

                    //if conflict, merge them
                    FibonacciHeapNode<T> other = treeTable[current.Degree];
                    treeTable[current.Degree] = null;

                    FibonacciHeapNode<T> smaller = current.Key > other.Key? other : current;
                    FibonacciHeapNode<T> bigger = current.Key > other.Key ? current : other;

                    bigger.Right.Left = bigger.Left;
                    bigger.Left.Right = bigger.Right;
                    bigger.Right = bigger.Left = bigger;
                    smaller.Child = Merge(smaller.Child, bigger);
                    bigger.Parent = smaller;
                    bigger.Marked = false;

                    //increase Degree
                    smaller.IncreaseDegree();

                    tocompare = smaller;
                }

                if (tocompare.Key > _minimumNode.Key)
                {
                    _minimumNode = current;
                }
            }
        }

        public bool Contains(T element)
        {
            if (element == null)
            {
                return false;
            }
            return _allElements.Contains(element);
        }

        public bool ContainsRange(ICollection<T> elements)
        {
            if (elements == null)
            {
                return false;
            }
            foreach (T element in elements)
            {
                if (!Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        public T DequeueMin()
        {
            _size--;

            //store old minimum node
            FibonacciHeapNode<T> oldMin = _minimumNode;

            if (_minimumNode.Right == _minimumNode)
            {
                _minimumNode = null;
            }
            else
            {
                _minimumNode.Left.Right = _minimumNode.Right;
                _minimumNode.Right.Left = _minimumNode.Left;
                _minimumNode = _minimumNode.Right;
            }

            //delete the parent reference of all childs of the old minimum node
            if (oldMin.Child != null)
            {
                FibonacciHeapNode<T> current = oldMin.Child;
                while (current != null)
                {
                    current.Parent = null;
                    current = current.Right;
                }
            }

            _minimumNode = Merge(_minimumNode, oldMin.Child);

            return oldMin.Element;
        }

        public bool IsEmpty()
        {
            return _minimumNode == null;
        }

        /// <summary>
        /// Merge two subheaps together.
        /// </summary>
        /// <param name="x">The parent of the first subheap.</param>
        /// <param name="y">The parent of the second subheap.</param>
        /// <returns>The merged heap.</returns>
        public FibonacciHeapNode<T> Merge(FibonacciHeapNode<T> x, FibonacciHeapNode<T> y)
        {
            if (x == null && y == null)
            {
                return null;
            }
            else if (x != null && y == null)
            {
                return x;
            }
            else if (x == null && y != null)
            {
                return y;
            }
            else
            {
                FibonacciHeapNode<T> xOldRight = x.Right;
                x.Right = y.Right;
                x.Right.Left = x;
                y.Right = xOldRight;
                y.Right.Left = y;

                return x.Key >  y.Key? x : y;
            }
        }

        public T Min()
        {
            if (IsEmpty())
            {
                return default(T);
            }
            return _minimumNode.Element;
        }

        public int Potential()
        {
            return _trees + 2*_markedNodes;
        }

        

        private void MoveToRoot(FibonacciHeapNode<T> node)
        {
            if (IsEmpty())
            {
                _minimumNode = node;
            }
            else
            {
                node.Left.Right = node.Right;
                node.Right.Left = node.Left;

                node.Left = _minimumNode;
                node.Right = _minimumNode.Right;
                _minimumNode.Right = node;
                node.Right.Left = node;

                if (node.Key > _minimumNode.Key)
                {
                    _minimumNode = node;
                }
            }
        }
    }
}
