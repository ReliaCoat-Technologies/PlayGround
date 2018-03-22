using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ANTLRWorkspace
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "1 + (2 + 3^2) + 4";

            Console.WriteLine(input);

            var inputStream = new AntlrInputStream(input);
            var lexer = new MathExpressionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MathExpressionParser(tokenStream);

            var parseFormulaTree = parser.expression();
            var generator = new MathFormulaGenerator();
            var value = generator.Visit(parseFormulaTree);

            Console.WriteLine(value);
            Console.ReadLine();
        }
    }

    public class MathFormulaGenerator : MathExpressionBaseVisitor<double>
    {
        public override double VisitExpression(MathExpressionParser.ExpressionContext context)
        {
            return getNodeValue(context);
        }

        public override double VisitTerm(MathExpressionParser.TermContext context)
        {
            return getNodeValue(context);
        }

        public override double VisitFactor(MathExpressionParser.FactorContext context)
        {
            return getNodeValue(context);
        }

        public override double VisitParentheses(MathExpressionParser.ParenthesesContext context)
        {
            var expressionContext = context.children[1] as MathExpressionParser.ExpressionContext;

            if (expressionContext == null)
                throw new ArgumentException($"Invalid term in parentheses. {context.GetText()}");

            return VisitExpression(expressionContext);
        }

        public override double VisitChildren(IRuleNode node)
        {
            double value = 0;

            if (node is MathExpressionParser.TermContext)
                value = VisitTerm((MathExpressionParser.TermContext)node);
            else if (node is MathExpressionParser.FactorContext)
                value = VisitFactor((MathExpressionParser.FactorContext)node);
            else if (node is MathExpressionParser.ExpressionContext)
                value = VisitExpression((MathExpressionParser.ExpressionContext)node);
            else if (node is MathExpressionParser.ParenthesesContext)
                value = VisitParentheses((MathExpressionParser.ParenthesesContext)node);
            else
                throw new ArgumentException($"Invalid parser context: {node.GetType()}");

            return value;
        }

        public double getNodeValue(IRuleNode context, Func<double, double, double> mathFunc = null)
        {
            if (context.ChildCount % 2 == 0)
                throw new ArgumentException("Invalid number of arguments. Should be an odd number (i.e. \"A + B + C\" has 5 arguments");
            else if (context.ChildCount == 1)
                return getAtomicValue(context);
            else
                return doArithmetic(context, mathFunc);
        }

        private double getAtomicValue(IRuleNode context)
        {
            double value;

            if (!double.TryParse(context.GetChild(0).GetText(), out value))
            {
                value = VisitChildren(context.GetChild(0) as IRuleNode);
            }

            return value;
        }

        private double doArithmetic(IRuleNode context, Func<double, double, double> mathFunc)
        {
            var value = getAtomicValue(context);

            for (var i = 1; i < context.ChildCount; i += 2)
            {
                double value2;
                if (!double.TryParse(context.GetChild(i + 1).GetText(), out value2))
                {
                    value2 = VisitChildren(context.GetChild(i + 1) as IRuleNode);
                }

                if (mathFunc != null)
                    return mathFunc.Invoke(value, value2);

                var mathOperator = context.GetChild(i).GetText();

                switch (mathOperator)
                {
                    case "+":
                        value += value2;
                        break;
                    case "-":
                        value -= value2;
                        break;
                    case "*":
                        value *= value2;
                        break;
                    case "/":
                        value /= value2;
                        break;
                    case "^":
                        value = Math.Pow(value, value2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid Math Operator {mathOperator}");
                }
            }

            return value;
        }
    }
}
