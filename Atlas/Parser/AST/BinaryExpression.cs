namespace Atlas.Parser.AST
{
    public class BinaryExpression : Expression
    {
        public string ColumnName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
