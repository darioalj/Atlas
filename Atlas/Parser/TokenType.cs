namespace Atlas.Parser
{
    public enum TokenType
    {
        DataType,
        Keyword,
        Identifier,
        Number,
        String,

        OpenParen,
        CloseParen,

        Comma,
        Semicolon,

        Equals,
        NotEquals,
        Greater,
        Lesser,
        GreaterEquals,
        LesserEquals,
        Star,

        EOF
    }
}
