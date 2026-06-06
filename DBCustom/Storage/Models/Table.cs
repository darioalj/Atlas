namespace Atlas.Storage.Models
{
    public class Table
    {
        public string Name { get; set; }
        public List<Row> Rows { get; set; }
        public List<Column> Columns { get; set; }

        public Table(string name)
        {
            Name = name;
            Rows = [];
            Columns = [];
        }

        public Table(string name, List<Row> rows, List<Column> columns)
        {
            Name = name;
            Rows = rows;
            Columns = columns;
        }
    }
}
