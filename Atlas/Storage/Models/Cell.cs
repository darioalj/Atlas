using Atlas.Common;

namespace Atlas.Storage.Models
{
    public class Cell
    {
        public required object Value { get; set; }
        public DataType Type { get; set; }
    }
}
