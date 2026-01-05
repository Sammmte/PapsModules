using System.Collections.Generic;
using System.Linq;

namespace Paps.ValueReferences.Editor
{
    public class TreeNode<TData>
    {
        public string Name { get; set; }
        public List<TreeNode<TData>> Children { get; set; }
        public TreeNode<TData> Parent { get; set; }
        public TData Data { get; set; }

        public TreeNode(string name)
        {
            Name = name;
            Children = new List<TreeNode<TData>>();
        }

        public TreeNode<TData> AddChild(string childName)
        {
            var childNode = new TreeNode<TData>(childName);
            childNode.Parent = this;
            Children.Add(childNode);
            return childNode;
        }

        internal void AddChild(TreeNode<TData> node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public TreeNode<TData> GetOrAddChild(string childName)
        {
            var existingChild = Children.FirstOrDefault(c => c.Name == childName);
            if (existingChild != null)
                return existingChild;

            return AddChild(childName);
        }

        public string GetPath()
        {
            var path = Name;

            var currentNode = Parent;

            while(currentNode != null && currentNode.Parent != null)
            {
                path = path.Insert(0, $"{currentNode.Name}/");
                currentNode = currentNode.Parent;
            }

            return path;
        }
    }
}
