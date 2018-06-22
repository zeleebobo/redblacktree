using System;
using System.Linq;
using RedBlackTree;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new RedBlackTree<int>(15, 5, 1, 11, 14, 8, 22, 9, 3);

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
            var leafs = nodes.Where(x => x.IsLeaf).ToList();
            var heights = leafs.Select(getBlackHeight).ToList();

            Console.WriteLine(heights.Distinct().Count());
        }
    }
}