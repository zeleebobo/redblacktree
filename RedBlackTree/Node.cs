using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace RedBlackTree
{
    public class Node<TValue> where TValue: IComparable<TValue>
    {
        private Node<TValue> left;
        private Node<TValue> right;
        private bool isBlack;
        
        private Node()
        {
            left = NilLeaf;
            right = NilLeaf;
            isBlack = false;
        }

        public Node(TValue value) : this()
        {
            Value = value;
        }
        
        public Node<TValue> Left
        {
            get => left;
            set
            {
                left = value;
                if (left != null && left != NilLeaf)
                {
                    left.Parent = this;
                }
            }
        }
        
        public Node<TValue> Right
        {
            get => right;
            set
            {
                right = value;
                if (right != null && right != NilLeaf)
                {
                    right.Parent = this;
                }
            }
        }

        public Node<TValue> Parent { get; private set; }

        internal Node<TValue> Sibling
        {
            get
            {
                if (Parent == null)
                    return null;
                return IsLeft ? Parent.Right : Parent.Left;
            }
        }

        public bool IsRed => !isBlack;
        public bool IsBlack => isBlack;
        public bool IsNil => this == NilLeaf;
        public bool IsLeft => Parent != null && Parent.Left == this;
        public bool IsRight => Parent != null && Parent.Right == this;
        public bool IsLeaf => Left == NilLeaf && Right == NilLeaf;
        

        internal Node<TValue> SetBlack()
        {
            isBlack = true;
            return this;
        }

        internal Node<TValue> SetRed()
        {
            isBlack = false;
            return this;
        }
        
        internal void UnbindFromParent()
        {
            if (Parent == null)
                return;
            if (IsLeft)
                Parent.Left = NilLeaf;
            else
                Parent.Right = NilLeaf;
            Parent = null;
        }

        internal Node<TValue> SetSameColor(Node<TValue> source) => source.IsBlack ? SetBlack() : SetRed();
        
        public TValue Value { get; }
        
        internal static Node<TValue> NilLeaf = new Node<TValue>(){ isBlack = true};
    }
    
    
}