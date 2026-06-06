using Atlas.Parser.AST;
using Atlas.Storage.Models;

namespace Atlas.Storage
{
    public interface IStorageEngine
    {
        void CreateTable(string tableName, List<Column> columns);

        List<Column> GetColumns(string tableName);
        Table? GetTable(string tableName);

        void Insert(string tableName, Row row);

        List<Row> Select(string tableName, Expression? where);

        void Flush();
    }
}
