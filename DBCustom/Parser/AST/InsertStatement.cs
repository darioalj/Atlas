namespace Atlas.Parser.AST
{
    public class InsertStatement : Statement
    {
        public required string TableName { get; set; }
        public required RowDefinition Values { get; set; }
    }
}
