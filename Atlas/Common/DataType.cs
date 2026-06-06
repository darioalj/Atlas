namespace Atlas.Common
{
    public enum DataType
    {
        Int,
        String
    }

    public static class DataTypeExtensions
    {
        public static DataType Parse(string type)
        {
            return type.ToUpper() switch
            {
                "INT" => DataType.Int,
                "STRING" => DataType.String,
                _ => throw new Exception($"Unknown type '{type}'")
            };
        }
    }
}