using Atlas.Common;

namespace Atlas.Storage.Models
{
    public class Column
    {
        public required string Name { get; set; }
        public DataType Type { get; set; }
    }
}
