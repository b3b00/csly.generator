
using csly.models;
using System;

namespace <#NS#>
{


    public class Squasher<IN, OUT> where IN : struct, Enum
    {

        public Squasher()
        {
        }

        public SyntaxNode<IN, OUT> Squash(SyntaxNode<IN, OUT> node)
        {
            if (node == null)
            {
                return node;
            }

            // First, recursively squash all children
            for (int i = 0; i < node.Children.Count; i++)
            {
                if (node.Children[i] is SyntaxNode<IN, OUT> childNode)
                {
                    node.Children[i] = Squash(childNode);
                }
            }

            // If this node is a bypass node, replace it with its first child
            if (node.IsByPassNode && node.Children.Count > 0)
            {
                if (node.Children[0] is SyntaxNode<IN, OUT> firstChild)
                {
                    return firstChild;
                }
            }

            return node;
        }

        public SyntaxNode<IN, OUT> FixAssoc(SyntaxNode<IN, OUT> node)
        {
            // walk the tree and remove bypass nodes
            if (node == null)
            {
                return node;
            }

            // First, recursively squash all children
            for (int i = 0; i < node.Children.Count; i++)
            {
                if (node.Children[i] is SyntaxNode<IN, OUT> childNode)
                {
                    node.Children[i] = FixAssoc(childNode);
                }
            }

            var isLeftAssoc = node.IsBinaryOperationNode && node.IsLeftAssociative
                                                          && node.Right is SyntaxNode<IN, OUT> right && right.IsExpressionNode
                                                          && right.Precedence == node.Precedence;

            if (isLeftAssoc)
            {
                // If this node is a bypass node, replace it with its first child
                var newLeft = node;
                var newTop = (SyntaxNode<IN, OUT>)node.Right;
                newLeft.Right = newTop.Left;
                newTop.Left = newLeft;
                node = newTop;
            }

            return node;
        }
    }
}