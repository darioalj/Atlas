using Atlas.Common;
using Atlas.Parser.AST;
using Atlas.Storage;
using Atlas.Storage.Models;

namespace Atlas.Executor
{
    public class Executor
    {
        private readonly IStorageEngine _storage;

        public Executor(IStorageEngine storage)
        {
            _storage = storage;
        }

        public object? Execute(Statement statement)
        {
            switch (statement)  
            {
                case CreateTableStatement create:
                    {
                        var columns = create.Columns.Select(c => new Column
                        {
                            Name = c.Name,
                            Type = DataTypeExtensions.Parse(c.Type)
                        }).ToList();

                        _storage.CreateTable(create.TableName, columns);
                        return $"{create.TableName} table created successfully";
                    }

                case InsertStatement insert:
                    {
                        var columns = _storage.GetColumns(insert.TableName);
                        var row = ParseRow(insert.TableName, insert.Values, columns);

                        _storage.Insert(insert.TableName, row);
                        return $"Values inserted into {insert.TableName} successfully";
                    }
                case SelectStatement select:
                    {
                        var result = _storage.Select(select.TableName, select.Where);
                        return new QueryResult
                        {
                            Columns = _storage.GetColumns(select.TableName),
                            Rows = result
                        };
                    }
                default:
                    throw new Exception($"Unsupported statement type '{statement.GetType().Name}'.");
            }
        }

        private Row ParseRow(string tableName, RowDefinition row, List<Column> columns)
        {
            if (row.Cells.Count != columns.Count)
            {
                throw new Exception($"INSERT into '{tableName}' provided {row.Cells.Count} values, but the table expects {columns.Count} columns.");
            }

            var result = new List<Cell>();

            for (int i = 0; i < columns.Count; i++)
            {
                var cell = row.Cells[i];
                var cellType = DataTypeExtensions.Parse(cell.Type);

                var column = columns[i];
                
                if (cellType != column.Type)
                {
                    throw new Exception($"Type mismatch in column '{column.Name}': expected {column.Type}, got {cellType}.");
                }

                result.Add(new Cell
                {
                    Value = cell.Value,
                    Type = cellType
                });
            }

            return new Row
            {
                Values = result
            };
        }
    }
}
