using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    abstract class Node
    {
        public abstract double GenerateNumber();
    }

    class NumberNode : Node
    {
        public double Value { get; private set; }

        public NumberNode(double value)
        {
            this.Value = value;
        }

        public override double GenerateNumber()
        {
            return this.Value;
        }
    }

    class BinaryOpNode : Node
    {
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        public TokenType Op { get; private set; }

        public BinaryOpNode(Node left, Node right, TokenType op)
        {
            this.Left = left;
            this.Right = right;
            this.Op = op;
        }

        public override double GenerateNumber()
        {
            double leftValue = this.Left.GenerateNumber();
            double rightValue = this.Right.GenerateNumber();

            switch (this.Op)
            {
                case TokenType.PLUS: return leftValue + rightValue;
                case TokenType.MINUS: return leftValue - rightValue;
                case TokenType.MULTIPLY: return leftValue * rightValue;
                case TokenType.DIVIDE: return leftValue / rightValue;
                case TokenType.MODULO: return leftValue % rightValue;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }
    }

    class SyntaxErrorException : Exception { }

    class Parser
    {
        private Lexer lexer;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        public Node ParseOperand()
        {
            if (lexer.CurrentToken == TokenType.NUMBER)
            {
                double value = lexer.CurrentNumber;
                lexer.NextToken();
                return new NumberNode(value);
            }
            else if (lexer.CurrentToken == TokenType.OPEN_PAREN)
            {
                lexer.NextToken();

                Node result = Parse();

                if (lexer.CurrentToken != TokenType.CLOSE_PAREN)
                {
                    throw new SyntaxErrorException();
                }
                else
                {
                    lexer.NextToken();
                }

                return result;
            }

            Debug.Assert(false);
            return null;
        }

        public Node ParseMul()
        {
            Node left = ParseOperand();
            while (lexer.CurrentToken == TokenType.MULTIPLY ||
                lexer.CurrentToken == TokenType.DIVIDE ||
                lexer.CurrentToken == TokenType.MODULO)
            {
                TokenType op = lexer.CurrentToken;
                lexer.NextToken();

                Node right = ParseOperand();

                left = new BinaryOpNode(left, right, op);
            }

            return left;
        }

        public Node ParseAdd()
        {
            Node left = ParseMul();
            while (lexer.CurrentToken == TokenType.PLUS ||
                lexer.CurrentToken == TokenType.MINUS)
            {
                TokenType op = lexer.CurrentToken;
                lexer.NextToken();

                Node right = ParseMul();

                left = new BinaryOpNode(left, right, op);
            }

            return left;
        }

        public Node Parse()
        {
            return ParseAdd();
        }
    }
}
