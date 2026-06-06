using Atlas.Storage.Models;

namespace Atlas.Executor
{
    public class QueryResult
    {
        public List<Column> Columns { get; set; }

        public List<Row> Rows { get; set; }
    }
}
