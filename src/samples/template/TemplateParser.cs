using csly.template.models;
using System.Collections.Generic;
using template;
using template.model;
using template.model.expressions;

namespace template;


    [ParserRoot("template")]
    public class TemplateParser
    {
        
        #region structure

        [Production("template: item*")]
        public ITemplate Template(List<ITemplate> items)
        {
            return new Template(items);
        }

        [Production("item : TEXT")]
        public ITemplate Text(Token<TemplateLexer> text)
        {
         return new Text(text.Value);
        }

        [Production("item :OPEN_VALUE[d] TemplateParser_expressions CLOSE_VALUE[d]")]
        public ITemplate Value(ITemplate value) => value;
        
        [Production("item : OPEN_CODE[d] IF[d] OPEN_PAREN[d] TemplateParser_expressions CLOSE_PAREN[d] CLOSE_CODE[d] item* elseBlock? OPEN_CODE[d] ENDIF[d] CLOSE_CODE[d] ")]
        public ITemplate Conditional(ITemplate cond, List<ITemplate> thenBlock, ValueOption<ITemplate> elseBlock)
        {
            var ifthenelse = new IfThenElse(cond as Expression, new Block(thenBlock), elseBlock.Match(x => x, () => new Block()) as Block);
            return ifthenelse;
        }

        [Production("if : OPEN_CODE[d] IF[d] OPEN_PAREN[d] TemplateParser_expressions CLOSE_PAREN[d] CLOSE_CODE[d]")]
        public ITemplate If(ITemplate condition)
        {
            return condition;
        }

        [Production("elseBlock : OPEN_CODE[d] ELSE[d] CLOSE_CODE[d] item*")]
        public ITemplate elseBlock(List<ITemplate> items)
        {
            return new Block(items);
        }
        

        [Production("item : OPEN_CODE[d] FOR[d] INT RANGE[d] INT AS[d] ID CLOSE_CODE[d] item* OPEN_CODE[d] END[d] CLOSE_CODE[d]")]
        public ITemplate fori(Token<TemplateLexer> start, Token<TemplateLexer> end, Token<TemplateLexer> iterator, List<ITemplate> items)
        {
            return new ForI(start.IntValue, end.IntValue, iterator.Value, items);
        }
        
        [Production("item : OPEN_CODE[d] FOR[d] ID AS[d] ID CLOSE_CODE[d] item* OPEN_CODE[d] END[d] CLOSE_CODE[d]")]
        public ITemplate _foreach(Token<TemplateLexer> listName, Token<TemplateLexer> iterator, List<ITemplate> items)
        {
            return new ForEach(listName.Value, iterator.Value, items);
        }
       
        #endregion
        
        #region COMPARISON OPERATIONS

        [Infix("LESSER", Associativity.Right, 50)]
        [Infix("GREATER", Associativity.Right, 50)]
        [Infix("EQUALS", Associativity.Right, 50)]
        [Infix("DIFFERENT", Associativity.Right, 50)]
        public ITemplate binaryComparisonExpression(ITemplate left, Token<TemplateLexer> operatorToken,
            ITemplate right)
        {
            
            var oper = BinaryOperator.EQUALS;

            oper = operatorToken.TokenID switch
            {
                TemplateLexer.LESSER => BinaryOperator.LESSER,
                TemplateLexer.GREATER => BinaryOperator.GREATER,
                TemplateLexer.EQUALS => BinaryOperator.EQUALS,
                TemplateLexer.DIFFERENT => BinaryOperator.DIFFERENT,
                _ => BinaryOperator.EQUALS
            };

            return new BinaryOperation(left as Expression, oper, right as Expression);
        }

        #endregion

        #region STRING OPERATIONS

         [Operation((int) TemplateLexer.CONCAT, Affix.InFix, Associativity.Right, 10)]
         public ITemplate binaryStringExpression(ITemplate left, Token<TemplateLexer> operatorToken, ITemplate right)
         {
             return new BinaryOperation(left as Expression, BinaryOperator.CONCAT, right as Expression);
         }

        #endregion
        
          #region OPERANDS

          
        [Production("primary: INT")]
        public ITemplate PrimaryInt(Token<TemplateLexer> intToken)
        {
            return new IntegerConstant(intToken.IntValue, intToken.Position);
        }

        
        [Production("primary: TRUE")]
        [Production("primary: FALSE")]
        public ITemplate PrimaryBool(Token<TemplateLexer> boolToken)
        {
            return new BoolConstant(bool.Parse(boolToken.StringWithoutQuotes) ? true : false);
        }

        
        [Production("primary: STRING")]
        public ITemplate PrimaryString(Token<TemplateLexer> stringToken)
        {
            return new StringConstant(stringToken.StringWithoutQuotes, stringToken.Position);
        }

        
        [Production("primary: ID")]
        public ITemplate PrimaryId(Token<TemplateLexer> varToken)
        {
            return new Variable(varToken.Value);
        }

        [Production("primary: OPEN_PAREN[d] TemplateParser_expressions CLOSE_PAREN[d]")]
        public ITemplate PrimaryParens(ITemplate expr)
        {
            return expr;
        }

    [Operand]
        [Production("operand: primary")]
        public ITemplate Operand(ITemplate prim)
        {
            return prim;
        }

        #endregion

        #region NUMERIC OPERATIONS

        [Operation((int) TemplateLexer.PLUS, Affix.InFix, Associativity.Right, 10)]
        [Operation((int) TemplateLexer.MINUS, Affix.InFix, Associativity.Right, 10)]
        public ITemplate binaryTermNumericExpression(ITemplate left, Token<TemplateLexer> operatorToken,
            ITemplate right)
        {
            var oper = BinaryOperator.ADD;

            oper = operatorToken.TokenID switch
            {
                TemplateLexer.PLUS => BinaryOperator.ADD,
                TemplateLexer.MINUS => BinaryOperator.SUB,
                _ => BinaryOperator.ADD
            };

            return new BinaryOperation(left as Expression, oper, right as Expression);
        }

        [Operation((int) TemplateLexer.TIMES, Affix.InFix, Associativity.Right, 50)]
        [Operation((int) TemplateLexer.DIVIDE, Affix.InFix, Associativity.Right, 50)]
        public ITemplate binaryFactorNumericExpression(ITemplate left, Token<TemplateLexer> operatorToken,
            ITemplate right)
        {
            var oper = BinaryOperator.ADD;

            oper = operatorToken.TokenID switch
            {
                TemplateLexer.TIMES => BinaryOperator.MULTIPLY,
                TemplateLexer.DIVIDE => BinaryOperator.DIVIDE,
                _ => BinaryOperator.MULTIPLY
            };

            return new BinaryOperation(left as Expression, oper, right as Expression);
        }

        [Prefix((int) TemplateLexer.MINUS, Associativity.Right, 100)]
        public ITemplate unaryNumericExpression(Token<TemplateLexer> operation, ITemplate value)
        {
            return new Neg(value as Expression, operation.Position);
        }

        #endregion


        #region BOOLEAN OPERATIONS

        [Operation((int) TemplateLexer.OR, Affix.InFix, Associativity.Right, 10)]
        public ITemplate binaryOrExpression(ITemplate left, Token<TemplateLexer> operatorToken, ITemplate right)
        {
            return new BinaryOperation(left as Expression, BinaryOperator.OR, right as Expression);
        }

        [Operation((int) TemplateLexer.AND, Affix.InFix, Associativity.Right, 50)]
        public ITemplate binaryAndExpression(ITemplate left, Token<TemplateLexer> operatorToken, ITemplate right)
        {
            return new BinaryOperation(left as Expression, BinaryOperator.AND, right as Expression);
        }

        [Operation((int) TemplateLexer.NOT, Affix.PreFix, Associativity.Right, 100)]
        public ITemplate binaryOrExpression(Token<TemplateLexer> operatorToken, ITemplate value)
        {
            return new Not(value as Expression,operatorToken.Position);
        }

        #endregion
    }