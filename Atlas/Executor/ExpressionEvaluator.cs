using Atlas.Common;
using Atlas.Parser.AST;
using Atlas.Storage.Models;

namespace Atlas.Executor
{
    public static class ExpressionEvaluator
    {
        public static bool Evaluate(Expression expr, Row row, Table table)
        {
            if (expr is BinaryExpression be)
            {
                var columnIndex = table.Columns.FindIndex(c => c.Name == be.ColumnName);

                var cell = row.Values[columnIndex];

                return EvaluateBinary(cell, be.Operator, be.Value);
            }

            throw new Exception("Unknown expression");
        }

        private static bool EvaluateBinary(Cell cell, string op, string right)
        {
            if (cell.Type == DataType.String)
            {
                string left = cell.Value.ToString();

                return op switch
                {
                    "=" => left == right,
                    "!=" => left != right,
                    _ => throw new Exception($"Unknown operator {op}")
                };
            } 
            else if (cell.Type == DataType.Int)
            {
                int leftInt = int.Parse(cell.Value.ToString());
                int rightInt = int.Parse(right);

                return op switch
                {
                    "=" => leftInt == rightInt,
                    "!=" => leftInt != rightInt,
                    ">" => leftInt > rightInt,
                    "<" => leftInt < rightInt,
                    ">=" => leftInt >= rightInt,
                    "<=" => leftInt <= rightInt,
                    _ => throw new Exception($"Unknown operator {op}")
                };
            }

            throw new Exception($"Unknown operator {op}");
        }
    }
}
