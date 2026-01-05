using UnityEngine;
using System;
using System.Text;

namespace Paps.ValueReferences.Editor
{
    public class PathTree : PathTree<object> { }

    public class PathTree<TData>
    {
        public TreeNode<TData> Root { get; private set; }

        internal PathTree()
        {
            Root = new TreeNode<TData>("Root");
        }

        internal PathTree(TreeNode<TData> root)
        {
            Root = root;
        }

        public void Traverse(Action<TreeNode<TData>> traverseAction)
        {
            Traverse(Root, traverseAction);
        }

        private void Traverse(TreeNode<TData> node, Action<TreeNode<TData>> traverseAction)
        {
            traverseAction(node);

            for(int i = 0; i < node.Children.Count; i++)
            {
                Traverse(node.Children[i], traverseAction);
            }
        }

        public void Print()
        {
            var stringBuilder = new StringBuilder();
            GetPrintString(Root, stringBuilder);

            Debug.Log(stringBuilder.ToString());
        }

        private void GetPrintString(TreeNode<TData> node, StringBuilder stringBuilder, int level = 0)
        {
            if (node == null) return;

            stringBuilder.Append($"{new string('-', level * 2)}[{node.Name}]\n");
            foreach (var child in node.Children)
            {
                GetPrintString(child, stringBuilder, level + 1);
            }
        }

        public static PathTree<TData> BuildFromPaths(string[] paths, char separator = '/')
        {
            var pathTree = new PathTree<TData>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var parts = path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var currentNode = pathTree.Root;

                foreach (var part in parts)
                {
                    currentNode = currentNode.GetOrAddChild(part);
                }
            }

            return pathTree;
        }
    }
}
