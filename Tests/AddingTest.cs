using System;
using Xunit;
using RedBlackTree;

namespace Tests
{
    public class AddingTest
    {
        [Fact]
        public void AddOneItem()
        {
            var tree = new RedBlackTree<int>(1);
            Assert.NotNull(tree.Root);
            Assert.Equal(1, tree.Root.Value);
        }

        [Fact]
        public void AddItems()
        {
            var array = new []{ 1, 2, 3, 4};
            var tree = new RedBlackTree<int>(array);
            Assert.Equal(tree.Count, array.Length);
            Assert.Contains(1, tree);
            Assert.Contains(2, tree);
            Assert.Contains(3, tree);
            Assert.Contains(4, tree);            
        }

        [Fact]
        public void GetCollection()
        {
            var array = new []{ 1, 2, 3, 4};
            var tree = new RedBlackTree<int>(array);
            //Assert.Collection(tree);
        }        
    }
}