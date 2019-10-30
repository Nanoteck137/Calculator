using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    enum TokenType
    {
        UNKNOWN,

        NUMBER,
        IDENTIFIER,

        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        MODULO,

        EOS
    };

    class Lexer
    {
        private string text;
        private int ptr;

        public TokenType CurrentToken { get; private set; }
        public ulong CurrentNumber { get; private set; }

        public Lexer(string text)
        {
            this.text = text;
        }

        public void NextToken()
        {
            CurrentToken = TokenType.UNKNOWN;
            CurrentNumber = 0;

            if (ptr >= text.Length)
            {
                CurrentToken = TokenType.EOS;
                return;
            }

            char current = text[ptr];
            ptr++;

            switch (current)
            {
                case '+': CurrentToken = TokenType.PLUS; break;
                case '-': CurrentToken = TokenType.MINUS; break;
                case '*': CurrentToken = TokenType.MULTIPLY; break;
                case '/': CurrentToken = TokenType.DIVIDE; break;
                case '%': CurrentToken = TokenType.MODULO; break;
                default:
                    if (char.IsDigit(current))
                    {
                        CurrentNumber = (ulong)(current - '0');
                        while (ptr < text.Length && char.IsDigit(text[ptr]))
                        {
                            CurrentNumber *= 10;
                            CurrentNumber += (ulong)(text[ptr] - '0');

                            ptr++;
                        }

                        CurrentToken = TokenType.NUMBER;
                    }
                    break;
            }
        }
    }
}
