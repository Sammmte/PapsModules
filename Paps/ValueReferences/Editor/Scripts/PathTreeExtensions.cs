using System;

namespace Paps.ValueReferences.Editor
{
    public static class PathTreeExtensions
    {
        public static PathTree<TOutput> Map<TInput, TOutput>(this PathTree<TInput> source, Func<TreeNode<TInput>, TOutput> mapFunction)
        {
            return new PathTree<TOutput>(MapRecursively(source.Root, mapFunction));
        }

        private static TreeNode<TOutput> MapRecursively<TInput, TOutput>(TreeNode<TInput> inputNode, Func<TreeNode<TInput>, TOutput> mapFunction)
        {
            var newData = mapFunction(inputNode);

            var newNode = new TreeNode<TOutput>(inputNode.Name);
            newNode.Data = newData;

            foreach(var childNode in inputNode.Children)
            {
                newNode.AddChild(MapRecursively(childNode, mapFunction));
            }

            return newNode;
        }
    }
}
