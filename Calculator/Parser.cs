using System;
using System.Diagnostics;

namespace Calculator
{
    abstract class Node
    {
        /// <summary>
        /// Generate the a number from the inerited classes
        /// </summary>
        /// <returns>The number that the classes generates</returns>
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
            //NOTE(patrik): Parse Numbers, Power, Paren, Function operations

            if (lexer.CurrentToken == TokenType.NUMBER)
            {
                // Format 1: number
                //        2: number^number
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
            else if (lexer.CurrentToken == TokenType.MINUS)
            {
                // Format 1: -number
                lexer.NextToken();

                double value = lexer.CurrentNumber;
                lexer.ExpectToken(TokenType.NUMBER);

                return new NumberNode(-value);
            }
            else if (lexer.CurrentToken == TokenType.OPEN_PAREN)
            {
                // Format 1: (expr)
                lexer.NextToken();

                Node result = Parse();

                lexer.ExpectToken(TokenType.CLOSE_PAREN);

                return result;
            }
            else if (lexer.CurrentToken == TokenType.IDENTIFIER)
            {
                // Format 1: identifier
                //        2: identifier(expr, ...)
                string identifier = lexer.CurrentIdentifier;
                lexer.NextToken();

                if (lexer.CurrentToken == TokenType.OPEN_PAREN)
                {
                    lexer.NextToken();

                    Node node = Parse();

                    lexer.ExpectToken(TokenType.CLOSE_PAREN);

                    if (identifier.Equals("sqrt", StringComparison.OrdinalIgnoreCase))
                    {
                        SQRTNode result = new SQRTNode(node);
                        return result;
                    }
                }
            }

            return null;
        }

        public Node ParseMul()
        {
            // NOTE(patrik): Parse Multiply, Divide and modulo operation

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
            // NOTE(patrik): Parse Plus and minus operations

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
