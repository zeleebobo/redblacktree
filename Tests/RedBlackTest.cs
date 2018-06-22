using System;
using System.Linq;
using Xunit;
using RedBlackTree;

namespace Tests
{
    public class RedBlackTest
    {

        [Fact]
        void RootIsBlack2()
        {
            var tree = new RedBlackTree<int>(1, 2, 3, 4, 5, 6, 7);
            Assert.True(tree.Root.IsBlack);
            tree.Remove(2,3);
            Assert.True(tree.Root.IsBlack);
        }

        [Fact]
        void ItsTrueRedBlackTree()
        {
            var tree = new RedBlackTree<int>(15, 5, 1, 11, 14, 8, 22, 9, 3);
            
            Assert.True(tree.Root.IsBlack);

            int getBlackHeight<T>(Node<T> node) where T: IComparable<T>
            {
                var blackCount = 0;
                while (node.Parent != null)
                {
                    if (node.IsBlack)
                        blackCount++;
                    node = node.Parent;
                }
                return blackCount;
            }

            var nodes = tree.GetNodes().ToList();
            var heights = nodes.Where(x => x.IsLeaf).Select(getBlackHeight).ToArray();
            
            Assert.Equal(1, heights.Distinct().Count());
            
            Assert.All(nodes.Where(x => x.IsRed), x => Assert.True(x.Left.IsBlack && x.Right.IsBlack));
        }
    }
}