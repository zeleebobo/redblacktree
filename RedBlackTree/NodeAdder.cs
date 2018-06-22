using System;

namespace RedBlackTree
{
    internal class NodeAdder<TValue> where TValue: IComparable<TValue>
    {
        private RedBlackTree<TValue> tree;
        public NodeAdder(RedBlackTree<TValue> tree)
        {
            this.tree = tree;
        }
        
        internal void AddNode(ref Node<TValue> root, Node<TValue> newNode) 
        {
            
            if (root == null) {
                root = newNode;
                root.SetBlack();
                return;
            }                
            var node = root;
            while(true){
                if (node.Value.CompareTo(newNode.Value) > 0)  
                {
                    if (node.Left == Node<TValue>.NilLeaf) break;
                    node = node.Left;
                }
                else 
                {
                    if (node.Right == Node<TValue>.NilLeaf) break;
                    node = node.Right;
                }
            }

            if (node.Value.CompareTo(newNode.Value) > 0)
                node.Left = newNode;
            else 
                node.Right = newNode;

            FixTree(ref root, newNode); 
        }
        
        internal void FixTree(ref Node<TValue> root, Node<TValue> newNode)
        {
            var node = newNode;
            while(node.Parent != null && node.Parent.IsRed)
            {
                var parentNeighbor = node.Parent.Sibling;
                if (parentNeighbor != Node<TValue>.NilLeaf && parentNeighbor.IsRed)
                {
                    node.Parent.SetBlack();
                    parentNeighbor.SetBlack();
                    node.Parent.Parent.SetRed();
                    node = node.Parent.Parent;
                }
                else 
                {
                    if (node.Parent.IsLeft)
                    {
                        if (node.IsRight)
                        {
                            node = node.Parent;
                            tree.RotateLeft(node);
                        }
                        node.Parent.SetBlack();
                        node.Parent.Parent.SetRed();
                        tree.RotateRight(node.Parent.Parent);
                    }
                    else 
                    {
                        if (node.IsLeft)
                        {
                            node = node.Parent;
                            tree.RotateRight(node);
                        }
                        node.Parent.SetBlack();
                        node.Parent.Parent.SetRed();
                        tree.RotateLeft(node.Parent.Parent);
                    }
                }
            }
            root.SetBlack();
        }
    }
}