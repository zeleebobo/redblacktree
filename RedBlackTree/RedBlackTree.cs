using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Xsl;

namespace RedBlackTree
{
    public class RedBlackTree<TValue>: IEnumerable<TValue> where TValue: IComparable<TValue>
    {
        private Node<TValue> root;
        private NodeRemover<TValue> remover;
        private NodeAdder<TValue> adder;
        
        public RedBlackTree()
        {
            remover = new NodeRemover<TValue>(this);
            adder = new NodeAdder<TValue>(this);
        }

        public RedBlackTree(params TValue[] values) : this()
        {
            foreach (var value in values)
            {
                Add(value);
                Count++;
            }
        }

        public void Remove(params TValue[] values)
        {
            foreach (var value in values)
            {
                RemoveNodeWithValue(value);
                Count--;
            }
        }

        private void RemoveNodeWithValue(TValue value)
        {
            var delNode = FindNode(root, value);
            remover.RemoveNode(ref root, delNode);
        }

        public void Add(TValue value)
        {
            var newNode = new Node<TValue>(value);
            adder.AddNode(ref root, newNode);
        }

        /*internal void RotateLeft(Node<TValue> node)
        {
            var rChild = node.Right;
            node.Right = rChild.Left;
            var parent = node.Parent;
            
            if (parent == null)
            {
                root = rChild;
            }
            else if (node.IsLeft)
            {
                parent.Left = rChild;
            }
            else
            {
                parent.Right = rChild;
            }
            
            rChild.Left = node;
        }*/

        internal void RotateLeft(Node<TValue> node)
        {
            var rChild = node.Right;
            rChild.UnbindFromParent();
            if (node.Parent == null)
            {
                root = rChild;
                
            }
            else if (node.IsLeft)
            {
                node.Parent.Left = rChild;
            }
            else
            {
                node.Parent.Right = rChild;
            }

            node.Right = rChild.Left;
            rChild.Left = node;
        }
        
        internal void RotateRight(Node<TValue> node)
        {
            var lChild = node.Left;
            lChild.UnbindFromParent();
            if (node.Parent == null)
            {
                root = lChild;
            }
            else if (node.IsLeft)
            {
                node.Parent.Left = lChild;
            }
            else
            {
                node.Parent.Right = lChild;
            }

            node.Left = lChild.Right;
            lChild.Right = node;
        }

        internal Node<TValue> FindNode(Node<TValue> startNode, TValue value)
        {
            var node = startNode;
            while(true)
            {
                if (node == Node<TValue>.NilLeaf)
                    return null;
                if (node.Value.Equals(value))
                    return node;
                node = node.Value.CompareTo(value) < 0 ? node.Right : node.Left;
            }
        }
        
        private Node<TValue> GoToDeep(Node<TValue> start, Func<Node<TValue>, Node<TValue>> func)
        {
            if (root == null)
                return null;
            var node = root;
            while(func(node) != null)
                node = func(node);
            return node;
        }

        internal Node<TValue> Minimum(Node<TValue> start) => GoToDeep(start, x => x.Left);
        public TValue MinimumOrDefault => Minimum(root).Value;

        internal Node<TValue> Maximum(Node<TValue> start) => GoToDeep(start, x => x.Right);
        public TValue MaximumOrDefault => Maximum(root).Value;

        public IEnumerable<Node<TValue>> GetNodes()
        {
            if (root == null) yield break;
            var queue = new Queue<Node<TValue>>();
            queue.Enqueue(root);
            while(queue.Count != 0)
            {
                var node = queue.Dequeue();
                yield return node;
                if (!node.Left.IsNil)
                    queue.Enqueue(node.Left);
                if (!node.Right.IsNil)
                    queue.Enqueue(node.Right);
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            return GetNodes().Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Node<TValue> Root => root;
        
        public int Count { get; private set; }
    }
}