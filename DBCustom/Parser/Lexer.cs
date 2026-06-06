namespace Atlas.Parser
{
    public class Lexer
    {
        public static string[] ValidKeywords = ["SELECT", "FROM", "INSERT", "INTO", "VALUES", "CREATE", "TABLE", "WHERE"];
        public static string[] ValidDataTypes = ["INT", "STRING"];

        private string _text;
        private int _position;

        private char Current => _position < _text.Length ? _text[_position] : '\0';

        public Lexer(string text)
        {
            _text = text;
        }

        public List<Token> Tokenize()
        {
            var result = new List<Token>();

            while (Current != '\0')
            {
                if (char.IsWhiteSpace(Current))
                {
                    _position++;
                    continue;
                }

                if (char.IsLetter(Current))
                {
                    result.Add(ReadWord());
                    continue;
                }

                if (char.IsDigit(Current))
                {
                    result.Add(ReadNumber());
                    continue;
                }

                if (Current == '\'')
                {
                    result.Add(ReadString());
                    continue;
                }

                switch (Current)
                {
                    case '(':
                        result.Add(new Token(TokenType.OpenParen, "("));
                        break;

                    case ')':
                        result.Add(new Token(TokenType.CloseParen, ")"));
                        break;

                    case ',':
                        result.Add(new Token(TokenType.Comma, ","));
                        break;

                    case '*':
                        result.Add(new Token(TokenType.Star, "*"));
                        break;

                    case '=':
                        result.Add(new Token(TokenType.Equals, "="));
                        break;

                    case '!':
                        if(_text[_position + 1] == '=')
                        {
                            _position++;
                            result.Add(new Token(TokenType.NotEquals, "!="));
                            break;
                        }
                        result.Add(ReadWord());
                        break;

                    case '>':
                        if (_text[_position + 1] == '=')
                        {
                            _position++;
                            result.Add(new Token(TokenType.GreaterEquals, ">="));
                            break;
                        }

                        result.Add(new Token(TokenType.Greater, ">"));
                        break;

                    case '<':
                        if (_text[_position + 1] == '=')
                        {
                            _position++;
                            result.Add(new Token(TokenType.LesserEquals, "<="));
                            break;
                        }

                        result.Add(new Token(TokenType.Lesser, "<"));
                        break;

                    case ';':
                        result.Add(new Token(TokenType.Semicolon, ";"));
                        break;

                    default:
                        throw new Exception($"Unknown char: {Current}");
                }

                _position++;
            }

            result.Add(new Token(TokenType.EOF, ""));
            return result;
        }

        private Token ReadWord()
        {
            int start = _position;

            while (char.IsLetterOrDigit(Current))
            {
                _position++;
            }

            string word = _text[start.._position];

            if (ValidKeywords.Contains(word.ToUpper()))
            {
                return new Token(TokenType.Keyword, word);
            }
            else if (ValidDataTypes.Contains(word.ToUpper()))
            {
                return new Token(TokenType.DataType, word);
            }

            return new Token(TokenType.Identifier, word);
        }

        private Token ReadNumber()
        {
            int start = _position;

            while (char.IsDigit(Current))
            {
                _position++;
            }

            string value = _text[start.._position];

            return new Token(TokenType.Number, value);
        }

        private Token ReadString()
        {
            _position++;

            int start = _position;

            while (Current != '\'' && Current != '\0')
            {
                _position++;
            }

            string value = _text[start.._position];

            if (Current == '\'')
            {
                _position++;
            }

            return new Token(TokenType.String, value);
        }
    }
}
