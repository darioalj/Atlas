namespace Atlas.Parser.AST
{
    public class SelectStatement : Statement
    {
        public required string TableName { get; set; }
        public Expression? Where { get; set; }
    }
}
