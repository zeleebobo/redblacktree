using System;

namespace RedBlackTree
{
    internal static class NodeRemoverExtensions
    {
        internal static NodeRemover.DeleteState GetDeleteState<TValue>(this Node<TValue> node) where TValue: IComparable<TValue>
        {
            if (node.IsLeaf && node.IsRed)
                return NodeRemover.DeleteState.RedLeaf;

            if (node.IsLeaf && node.IsBlack && node.Parent != null)
            {
                if (node.Parent.IsRed)
                    return NodeRemover.DeleteState.BlackLeafRedParent;
                
                if (node.Sibling.IsRed)
                    return NodeRemover.DeleteState.BlackLeafBlackParentRedSibling;
                
                if (!node.Sibling.IsLeaf)
                    return NodeRemover.DeleteState.BlackLeafBlackParentSiblingNotLeaf;
                
                return NodeRemover.DeleteState.BlackLeafBlackParentSiblingLeaf;                        
            }                    

            if (!node.IsLeaf && !node.Left.IsLeaf && !node.Right.IsLeaf)
                return NodeRemover.DeleteState.AllChildsNotLeaf;

            if(!node.IsLeaf && node.IsBlack && node.Left.IsBlack && node.Right.IsBlack)
                return NodeRemover.DeleteState.BlackWithBlackLeafs;

            if (node.Left == Node<TValue>.NilLeaf && node.Right != Node<TValue>.NilLeaf || 
                node.Left != Node<TValue>.NilLeaf && node.Right == Node<TValue>.NilLeaf)
                return NodeRemover.DeleteState.OnlyOneChild;
                
            return NodeRemover.DeleteState.Error;
        }
    }

    internal class NodeRemover
    {
        internal enum DeleteState
        {
            RedLeaf,
            BlackLeafRedParent,
            BlackLeafBlackParentRedSibling,
            BlackLeafBlackParentSiblingNotLeaf,
            BlackLeafBlackParentSiblingLeaf,
            AllChildsNotLeaf,
            BlackWithBlackLeafs,
            OnlyOneChild,
            Error
        }
    }
    
    internal class NodeRemover<TValue> where TValue: IComparable<TValue>
    {
        private RedBlackTree<TValue> tree;
        public NodeRemover(RedBlackTree<TValue> redBlackTree) 
        {
            tree = redBlackTree;
        }
        
        internal void RemoveNode(ref Node<TValue> rootRef, Node<TValue> node)
        {
            if (rootRef == node && rootRef.IsLeaf)
            {
                rootRef = null;
                return;
            }
            
            switch(node.GetDeleteState())
            {
                case NodeRemover.DeleteState.RedLeaf:
                    node.UnbindFromParent();
                    break;

                case NodeRemover.DeleteState.BlackLeafRedParent:
                    DeleteBlackLeafRedParent(node);
                    break;

                case NodeRemover.DeleteState.BlackLeafBlackParentRedSibling:
                    DeleteBlackLeafBlackParentRedSibling(node);
                    break;

                case NodeRemover.DeleteState.BlackLeafBlackParentSiblingNotLeaf:
                    DeleteBlackLeafBlackParentSiblingNotLeaf(node);
                    break;

                case NodeRemover.DeleteState.BlackLeafBlackParentSiblingLeaf:
                    DeleteBlackLeafBlackParentSiblingLeaf(node);
                    break;

                case NodeRemover.DeleteState.AllChildsNotLeaf:
                    DeleteAllChildsNotLeaf(ref rootRef, node);
                    break;

                case NodeRemover.DeleteState.BlackWithBlackLeafs:
                    DeleteBlackWithBlackLeafs(ref rootRef, node);
                    break;

                case NodeRemover.DeleteState.OnlyOneChild:
                    DeleteNodeWithOnlyOneChild(ref rootRef, node);
                    break;

                case NodeRemover.DeleteState.Error:
                    throw new Exception("Incorrect delete state");
            }
        }

        private void DeleteNodeWithOnlyOneChild(ref Node<TValue> rootRef, Node<TValue> delNode)
        {
            var newNode = delNode.Left ?? delNode.Right;
            if (!newNode.IsRed) return;
            newNode.SetSameColor(delNode);
            FullTransplant(ref rootRef, delNode, newNode);
        }

        private void DeleteBlackWithBlackLeafs(ref Node<TValue> rootRef, Node<TValue> node)
        {
            var delSibling = node.Sibling;
            delSibling?.SetRed();
            var newNode = node.Left;
            FullTransplant(ref rootRef, node, newNode);
            newNode.Right.SetRed();
        }

        private void DeleteAllChildsNotLeaf(ref Node<TValue> rootRef, Node<TValue> node)
        {
            var minNode = tree.Minimum(node.Right);
            var minParent = minNode.Parent;
            var minRight = minNode.Right;
            minNode.Parent.Left = minRight;
            minNode.Right = Node<TValue>.NilLeaf;
            if (minNode.IsLeaf)
            {
                RemoveNode(ref rootRef, minNode);
                // DeleteLeaf(minNode);
            }

            minRight?.SetSameColor(minNode);

            minNode.UnbindFromParent();
            minNode.SetSameColor(node);
            FullTransplant(ref rootRef, node, minNode);
            if (minParent == node)
            {
                minNode.Right = minRight;
            }
        }

        private void DeleteBlackLeafBlackParentSiblingLeaf(Node<TValue> node)
        {
            node.Parent.SetRed();
            node.UnbindFromParent();
        }

        private void DeleteBlackLeafBlackParentSiblingNotLeaf(Node<TValue> node)
        {
            if (node.IsRight)
            {
                if (node.Sibling.Right != Node<TValue>.NilLeaf)
                {
                    node.Sibling.Right.SetBlack();
                    var delNeighbor = node.Sibling;
                    tree.RotateRight(node.Parent);
                    tree.RotateRight(node.Parent);
                    tree.RotateLeft(delNeighbor);
                }
                else
                {
                    node.Sibling.Left.SetBlack();
                    tree.RotateRight(node.Parent);
                    node.UnbindFromParent();
                }
            }
            else
            {
                if (node.Sibling.Left != Node<TValue>.NilLeaf)
                {
                    node.Sibling.Left.SetBlack();
                    var delNeighbor = node.Sibling;
                    tree.RotateLeft(node.Parent);
                    tree.RotateLeft(node.Parent);
                    tree.RotateRight(delNeighbor);
                }
                else
                {
                    node.Sibling.Right.SetBlack();
                    tree.RotateLeft(node.Parent);
                    node.UnbindFromParent();
                }
            }
        }

        private void DeleteBlackLeafBlackParentRedSibling(Node<TValue> node)
        {
            if (node.IsRight && !node.Sibling.Right.IsLeaf || node.IsLeft && !node.Sibling.Left.IsLeaf)
            {
                var neighbor = node.Sibling;
                if (node.IsRight)
                {
                    var grandsonOfNeighbor = neighbor.Right.Left;
                    grandsonOfNeighbor.SetBlack();
                    tree.RotateLeft(neighbor);
                    tree.RotateRight(node.Parent);
                    node.UnbindFromParent();
                    
                    if (grandsonOfNeighbor != Node<TValue>.NilLeaf) return;
                    var parentOfNeighbor = neighbor.Parent;
                    var neighborOfParentNeighbor = parentOfNeighbor.Sibling;
                    neighbor.SetBlack();
                    tree.RotateRight(parentOfNeighbor);
                    tree.RotateRight(neighborOfParentNeighbor);
                    tree.RotateLeft(parentOfNeighbor);
                }
                else 
                {
                    var grandsonOfNeighbor = neighbor.Left.Right;
                    grandsonOfNeighbor.SetBlack();
                    tree.RotateRight(neighbor);
                    tree.RotateLeft(node.Parent);
                    node.UnbindFromParent();
                    
                    if (grandsonOfNeighbor != Node<TValue>.NilLeaf) return;
                    var parentOfNeighbor = neighbor.Parent;
                    var neighborOfParentNeighbor = parentOfNeighbor.Sibling;
                    neighbor.SetBlack();
                    tree.RotateLeft(parentOfNeighbor);
                    tree.RotateLeft(neighborOfParentNeighbor);
                    tree.RotateRight(parentOfNeighbor);
                }
            }
            else
            {
                if (node.IsRight)
                {
                    node.Sibling.Right.SetRed();
                    node.Sibling.SetBlack();
                    tree.RotateRight(node.Parent);
                }
                else
                {
                    node.Sibling.Left.SetRed();
                    node.Sibling.SetBlack();
                    tree.RotateLeft(node.Parent);
                }
                node.UnbindFromParent();
            }  
        }

        private void DeleteBlackLeafRedParent(Node<TValue> node)
        {
            if (!node.Sibling.IsLeaf)
            {
                node.Parent.SetBlack();
                if (node.IsRight)
                {
                    tree.RotateLeft(node.Sibling);
                    tree.RotateRight(node.Parent);
                }
                else
                {
                    tree.RotateRight(node.Sibling);
                    tree.RotateLeft(node.Parent);
                }
            }
            else
            {
                node.Parent.SetBlack();
                node.Sibling.SetRed();
                node.UnbindFromParent();
            }
        }
        
        internal void FullTransplant(ref Node<TValue> rootRef, Node<TValue> oldNode, Node<TValue> newNode)
        {
            if (oldNode == Node<TValue>.NilLeaf || newNode == Node<TValue>.NilLeaf)
                throw new ArgumentNullException("Some of arguments is null");
            newNode.UnbindFromParent();
            if (oldNode == rootRef)
            {
                rootRef = newNode;
            }
            else if (oldNode.IsLeft)
            {
                oldNode.Parent.Left = newNode;
            }
            else 
            {
                oldNode.Parent.Right = newNode;
            }
            newNode.Left = oldNode.Left;
            newNode.Right = oldNode.Right;
        }
    }
}