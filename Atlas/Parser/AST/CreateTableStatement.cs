namespace Atlas.Parser.AST
{
    public class CreateTableStatement : Statement
    {
        public required string TableName { get; set; }
        public required List<ColumnDefinition> Columns { get; set; }
    }
}
