using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    abstract class Node { }

    class NumberNode : Node
    {
        public ulong Value { get; private set; }

        public NumberNode(ulong value)
        {
            this.Value = value;
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
            return null;
        }

        public Node ParseMul()
        {
            return null;
        }

        public Node ParseAdd()
        {
            return null;
        }

        public Node Parse()
        {
            return ParseAdd();
        }
    }
}
