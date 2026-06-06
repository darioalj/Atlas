using Atlas.Parser.AST;

namespace Atlas.Parser
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        
        private int _position;
        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public Statement Parse()
        {
            Statement statement;

            if (MatchKeyword("SELECT"))
            {
                statement = ParseSelect();
            }
            else if (MatchKeyword("INSERT"))
            {
                statement = ParseInsert();
            }
            else if (MatchKeyword("CREATE"))
            {
                statement = ParseCreateTable();
            }
            else
            {
                throw new Exception("Unknown statement");
            }

            Consume(TokenType.EOF);

            return statement;
        }

        private SelectStatement ParseSelect()
        {
            ConsumeKeyword("SELECT");

            Consume(TokenType.Star);

            ConsumeKeyword("FROM");

            string tableName = Consume(TokenType.Identifier).Value;

            Expression? where = null;

            if (MatchKeyword("WHERE"))
            {
                ConsumeKeyword("WHERE");
                where = ParseWhere();
            }

            return new SelectStatement
            {
                TableName = tableName,
                Where = where
            };
        }

        private InsertStatement ParseInsert()
        {
            ConsumeKeyword("INSERT");
            ConsumeKeyword("INTO");

            string tableName = Consume(TokenType.Identifier).Value;

            ConsumeKeyword("VALUES");

            Consume(TokenType.OpenParen);

            RowDefinition value = ConsumeValues();

            Consume(TokenType.CloseParen);

            return new InsertStatement
            {
                TableName = tableName,
                Values = value
            };
        }

        private CreateTableStatement ParseCreateTable()
        {
            ConsumeKeyword("CREATE");
            ConsumeKeyword("TABLE");

            string tableName = Consume(TokenType.Identifier).Value;

            Consume(TokenType.OpenParen);
            List<ColumnDefinition> columns = ConsumeColumns();
            Consume(TokenType.CloseParen);

            return new CreateTableStatement
            {
                TableName = tableName,
                Columns = columns
            };
        }

        private Expression ParseWhere()
        {
            var column = Consume(TokenType.Identifier).Value;

            var op = ParseComparisonOperator();

            var value = Advance().Value;

            return new BinaryExpression
            {
                ColumnName = column,
                Operator = op,
                Value = value
            };
        }

        private string ParseComparisonOperator()
        {
            var type = Current.Type;

            _position++;

            return type switch
            {
                TokenType.Equals => "=",
                TokenType.Greater => ">",
                TokenType.Lesser => "<",
                TokenType.GreaterEquals => ">=",
                TokenType.LesserEquals => "<=",
                TokenType.NotEquals => "!=",

                _ => throw new Exception($"Expected comparison operator but got {type}")
            };
        }

        private List<ColumnDefinition> ConsumeColumns()
        {
            var values = new List<ColumnDefinition>();

            while (true)
            {
                values.Add(ParseColumn());

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                    continue;
                }

                break;
            }

            return values;
        }

        private ColumnDefinition ParseColumn()
        {
            string name = Consume(TokenType.Identifier).Value;
            string dataType = Consume(TokenType.DataType).Value;

            return new ColumnDefinition()
            {
                Name = name,
                Type = dataType
            };
        }

        private RowDefinition ConsumeValues()
        {
            var values = new List<CellDefinition>();

            while (true)
            {
                values.Add(ParseValue());

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                    continue;
                }

                break;
            }

            return new RowDefinition
            {
                Cells = values
            };
        }

        private CellDefinition ParseValue()
        {
            if (Match(TokenType.Number))
            {
                string value = Consume(TokenType.Number).Value;

                return new CellDefinition
                {
                    Value = int.Parse(value),
                    Type = "INT"
                };
            }

            if (Match(TokenType.String))
            {
                return new CellDefinition
                {
                    Value = Consume(TokenType.String).Value,
                    Type = "STRING"
                };
            }

            throw new Exception(
                $"Expected Number or String but got {Current.Type}");
        }

        private bool MatchKeyword(string keyword)
        {
            return Match(TokenType.Keyword) && Current.Value.Equals(keyword, StringComparison.OrdinalIgnoreCase);
        }

        private bool Match(TokenType type)
        {
            return Current.Type == type;
        }

        private Token ConsumeKeyword(string keyword)
        {
            if (!MatchKeyword(keyword))
            {
                throw new Exception(
                    $"Expected keyword '{keyword}' but got '{Current.Value}'");
            }

            return Advance();
        }

        private Token Consume(TokenType expected)
        {
            if (Current.Type != expected)
            {
                throw new Exception($"Expected {expected} but got {Current.Type}");
            }

            return Advance();
        }

        private Token Advance()
        {
            return _tokens[_position++];
        }
    }
}
