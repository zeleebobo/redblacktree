using Xunit;
using RedBlackTree;

namespace Tests
{
    public class DeletingTest
    {
        [Fact]
        public void Delete()
        {
            var tree = new RedBlackTree<int>(1, 2, 3, 4, 5, 6, 7);
            Assert.Contains(2, tree);
            tree.Remove(2);
            Assert.DoesNotContain(2, tree);

            Assert.Contains(3, tree);
            Assert.Contains(6, tree);
            tree.Remove(3, 6);
            Assert.DoesNotContain(3, tree);
            Assert.DoesNotContain(6, tree);
        }

        [Fact]
        public void DeleteLeaf()
        {
            var tree = new RedBlackTree<int>(1, 2, 3, 4, 5, 6, 7);
            Assert.Contains(1, tree);
            tree.Remove(1);
            Assert.DoesNotContain(1, tree);
        }
    }
}