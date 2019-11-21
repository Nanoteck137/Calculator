using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Calculator
{
    class SyntaxErrorException : Exception { }

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

        POWER,

        OPEN_PAREN,
        CLOSE_PAREN,

        EOS
    };

    class Lexer
    {
        private string text;
        private int ptr;

        private StringBuilder builder;

        public TokenType CurrentToken { get; private set; }
        public double CurrentNumber { get; private set; }
        public string CurrentIdentifier { get; private set; }

        public Lexer(string text)
        {
            builder = new StringBuilder();

            Reset(text);
        }

        public void Reset(string text)
        {
            this.text = text;
            this.ptr = 0;

            NextToken();
        }

        private void ResetToken()
        {
            CurrentToken = TokenType.UNKNOWN;
            CurrentNumber = 0;
            CurrentIdentifier = "";
        }

        public void ExpectToken(TokenType expected)
        {
            if (CurrentToken != expected)
            {
                throw new SyntaxErrorException();
            }
            else
            {
                NextToken();
            }
        }

        public void NextToken()
        {
            ResetToken();

            if (ptr >= text.Length)
            {
                CurrentToken = TokenType.EOS;
                return;
            }

            int startPtr = ptr;
            char current = text[ptr];
            ptr++;

            switch (current)
            {
                case '+': CurrentToken = TokenType.PLUS; break;
                case '-': CurrentToken = TokenType.MINUS; break;
                case '*': CurrentToken = TokenType.MULTIPLY; break;
                case '/': CurrentToken = TokenType.DIVIDE; break;
                case '%': CurrentToken = TokenType.MODULO; break;
                case '^': CurrentToken = TokenType.POWER; break;
                case '(': CurrentToken = TokenType.OPEN_PAREN; break;
                case ')': CurrentToken = TokenType.CLOSE_PAREN; break;
                default:
                    // NOTE(patrik): Parse an identifier
                    if (char.IsLetter(current))
                    {
                        // Format 1: Hello_World123
                        builder.Append(current);

                        while (ptr < text.Length && (char.IsLetterOrDigit(text[ptr]) || text[ptr] == '_'))
                        {
                            builder.Append(text[ptr]);
                            ptr++;
                        }

                        CurrentIdentifier = builder.ToString();
                        CurrentToken = TokenType.IDENTIFIER;
                        builder.Clear();
                    }
                    // NOTE(patrik): Parse a number
                    else if (char.IsDigit(current))
                    {
                        // Format 1: 123 
                        //        2: 123.3
                        while (ptr < text.Length && (char.IsDigit(text[ptr]) || text[ptr] == '.'))
                        {
                            ptr++;
                        }

                        double res = double.Parse(text.Substring(startPtr, ptr - startPtr), CultureInfo.InvariantCulture);

                        CurrentNumber = res;
                        CurrentToken = TokenType.NUMBER;
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                    break;
            }
        }
    }
}
