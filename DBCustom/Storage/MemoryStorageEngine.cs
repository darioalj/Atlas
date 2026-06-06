using Atlas.Common;
using Atlas.Executor;
using Atlas.Parser.AST;
using Atlas.Storage.Models;

namespace Atlas.Storage
{
    public class MemoryStorageEngine : IStorageEngine
    {
        private readonly Dictionary<string, Table> _tables = [];
        private HashSet<string> _dirtyTables = [];

        public void CreateTable(string tableName, List<Column> columns)
        {
            if (_tables.ContainsKey(tableName))
            {
                throw new Exception($"Table '{tableName}' already exists");
            }

            var table = new Table(tableName);
            table.Columns = columns;

            _tables.Add(tableName, table);
            _dirtyTables.Add(tableName);

            Flush();
        }

        public int CountTables()
        {
            return _tables.Count;
        }

        public List<Column> GetColumns(string tableName)
        {
            var table = GetTableOrException(tableName);

            return table.Columns;
        }

        public void Insert(string tableName, Row values)
        {
            var table = GetTableOrException(tableName);

            table.Rows.Add(values);
            _dirtyTables.Add(tableName);
            
            Flush();
        }

        public List<Row> Select(string tableName, Expression? where)
        {
            var table = GetTableOrException(tableName);

            var result = table.Rows
                .Where(row =>
                    where == null ||
                    ExpressionEvaluator.Evaluate(where, row, table))
                .ToList();

            return result;
        }

        public Table GetTableOrException(string tableName)
        {
            return GetTable(tableName) ?? throw new Exception($"Table '{tableName}' does not exists");
        }

        public Table? GetTable(string tableName)
        {
            if (_tables.TryGetValue(tableName, out var table))
            {
                return table;
            }

            return null;
        }

        public void Flush()
        {
            foreach (var tableName in _dirtyTables)
            {
                SaveToDisk(tableName);    
            }

            _dirtyTables.Clear();
        }

        private void SaveToDisk(string tableName)
        {
            var table = GetTableOrException(tableName);

            var scheme = string.Join("|", table.Columns.Select(t => $"{t.Name}:{t.Type}"));
            var rows = table.Rows.Select(r => string.Join("|", r.Values.Select(c => c.Value)));

            Directory.CreateDirectory("data");
            File.WriteAllLines($"data/{tableName}.tbl", [
                scheme,
                ..rows
            ]);
        }

        public void LoadFromDisk()
        {
            Directory.CreateDirectory("data");

            var files = Directory.GetFiles("data", "*.tbl");

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);

                if (lines.Length == 0)
                    continue;

                var tableName = Path.GetFileNameWithoutExtension(file);

                var columns = lines[0]
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c =>
                    {
                        var parts = c.Split(':', StringSplitOptions.RemoveEmptyEntries);

                        return new Column
                        {
                            Name = parts[0],
                            Type = DataTypeExtensions.Parse(parts[1])
                        };
                    })
                    .ToList();

                var rows = lines
                    .Skip(1)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(line =>
                    {
                        var values = line
                            .Split('|', StringSplitOptions.None)
                            .Select((v, index) => new Cell
                            {
                                Value = v,
                                Type = columns[index].Type
                            })
                            .ToList();

                        return new Row
                        {
                            Values = values
                        };
                    })
                    .ToList();

                var table = new Table(tableName)
                {
                    Columns = columns,
                    Rows = rows
                };

                _tables.Add(tableName, table);
            }
        }
    }
}
