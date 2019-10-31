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

    class SQRTNode : Node
    {
        public Node Value { get; private set; }

        public SQRTNode(Node value)
        {
            this.Value = value;
        }

        public override double GenerateNumber()
        {
            double val = this.Value.GenerateNumber();
            return Math.Sqrt(val);
        }
    }

    class POWNode : Node
    {
        public double Value { get; private set; }
        public double Power { get; private set; }

        public POWNode(double value, double power)
        {
            this.Value = value;
            this.Power = power;
        }

        public override double GenerateNumber()
        {
            return Math.Pow(this.Value, this.Power);
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

                if (lexer.CurrentToken == TokenType.POWER)
                {
                    lexer.NextToken();

                    double power = lexer.CurrentNumber;
                    lexer.ExpectToken(TokenType.NUMBER);

                    return new POWNode(value, power);
                }
                return new NumberNode(value);
            }
            else if (lexer.CurrentToken == TokenType.OPEN_PAREN)
            {
                lexer.NextToken();

                Node result = Parse();

                lexer.ExpectToken(TokenType.CLOSE_PAREN);

                return result;
            }
            else if (lexer.CurrentToken == TokenType.IDENTIFIER)
            {
                string identifier = lexer.CurrentIdentifier;
                lexer.NextToken();

                if (lexer.CurrentToken == TokenType.OPEN_PAREN)
                {
                    lexer.NextToken();

                    Node node = Parse();
                    SQRTNode result = new SQRTNode(node);

                    lexer.ExpectToken(TokenType.CLOSE_PAREN);

                    return result;
                }
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
