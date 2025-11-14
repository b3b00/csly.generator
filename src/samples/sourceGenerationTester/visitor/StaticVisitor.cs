using csly.models;
using sourceGenerationTester.expressionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sourceGenerationTester.visitor
{
    internal class StaticVisitor
    {
        private readonly ExpressionParser _instance;

        public StaticVisitor(ExpressionParser instance)
        {
            _instance = instance;
        }
        //public enum Visitors
        //{
        //    Expression_0, //":term [PLUS|MINUS] expression"
        //    Expression_1, //":term"
        //    Term_0, //":factor"
        //    Term_1, //":factor [TIMES|DIVIDE] term"
        //    Factor_0, //":primary"
        //    Factor_1, //": MINUS primary"
        //    Primary_0, //":INT
        //    Primary_1, //":LPAREN expression RPAREN"

        //}

        public int VisitExpression(SyntaxNode<ExpressionToken,int> node)
        {
            switch(node.Visitor)
            {
                case nameof(Visitors.expression_0):
                    return VisitExpression_0(node);
                case nameof(Visitors.expression_1):
                    return VisitExpression_1(node);
                case nameof(Visitors.expression_2):
                    return VisitExpression_2(node);
                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return 0;
        }

        // expression : term
        private int VisitExpression_2(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitTerm((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            return _instance.Expression_Term(arg0);
        }

        // expression : term PLUS expression
        private int VisitExpression_0(SyntaxNode<ExpressionToken, int> node)
        {            
            var arg0 = VisitTerm((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<ExpressionToken, int>).Token;
            var arg2 = VisitExpression((SyntaxNode<ExpressionToken, int>)node.Children[2]);
            return _instance.Expression(arg0, arg1, arg2);
        }

        // expression : term MINUS expression
        private int VisitExpression_1(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitTerm((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<ExpressionToken, int>).Token;
            var arg2 = VisitExpression((SyntaxNode<ExpressionToken, int>)node.Children[2]);
            return _instance.Expression(arg0, arg1, arg2);
        }



        public int VisitTerm(SyntaxNode<ExpressionToken,int> node)
        {
            switch(node.Visitor)
            {
                case nameof(Visitors.term_0): // t : f TIME t
                    return VisitTerm_0(node); 
                case nameof(Visitors.term_1): // t : f DIVIDE t
                    return VisitTerm_1(node);
                case nameof(Visitors.term_2): // t : f
                    return VisitTerm_2(node);
                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return 0;
        }

        // term : factor
        private int VisitTerm_2(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitFactor((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            return _instance.Term_Factor(arg0);
        }

        private int VisitTerm_0(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitFactor((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<ExpressionToken, int>).Token;
            var arg2 = VisitTerm((SyntaxNode<ExpressionToken, int>)node.Children[2]);
            return _instance.Term(arg0, arg1, arg2);
        }

        // term : factor TIMES term
        private int VisitTerm_1(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitFactor((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<ExpressionToken, int>).Token;            
            var arg2 = VisitTerm((SyntaxNode<ExpressionToken, int>)node.Children[2]);
            return _instance.Term(arg0, arg1, arg2);
        }




        public int VisitFactor(SyntaxNode<ExpressionToken,int> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.factor_0):
                    return VisitFactor_0(node);
                case nameof(Visitors.factor_1):
                    return VisitFactor_1(node);
                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return 0;
        }

        // factor : primary
        private int VisitFactor_0(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = VisitPrimary((SyntaxNode<ExpressionToken, int>)node.Children[0]);
            return _instance.primaryFactor(arg0);
        }


        // factor : MINUS primary
        private int VisitFactor_1(SyntaxNode<ExpressionToken, int> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<ExpressionToken, int>).Token;
            var arg1 = VisitPrimary((SyntaxNode<ExpressionToken, int>)node.Children[1]);            
            return _instance.MinusFactor(arg0, arg1);
        }

        

        


        public int VisitPrimary(SyntaxNode<ExpressionToken, int> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.primary_0):
                    return VisitPrimary_0(node);// (SyntaxLeaf<ExpressionToken, int>)node);
                case nameof(Visitors.primary_1):
                    return VisitPrimary_1(node);
                default:
                    throw new NotImplementedException($"Visitor not implemented for primary non terminal");
            }
            return 0;
        }

        // primary : INT
        private int VisitPrimary_0(SyntaxNode<ExpressionToken, int> node)
        {
            return _instance.PrimaryInt((node.Children[0] as SyntaxLeaf<ExpressionToken, int>).Token);
        }


        // primary : LPAREN expression RPAREN
        private int VisitPrimary_1(SyntaxNode<ExpressionToken, int> node)
        {
            var leftP= node.Children[0] as SyntaxLeaf<ExpressionToken,int>;
            var rightP= node.Children[2] as SyntaxLeaf<ExpressionToken,int>;    
            var value = VisitExpression((SyntaxNode<ExpressionToken, int>)node.Children[1]);
            return _instance.Group(leftP.Token,value, rightP.Token);
        }
    }
}
