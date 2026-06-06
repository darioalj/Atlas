using Atlas.Executor;
using Atlas.Parser;
using Atlas.Storage;
using Atlas.Storage.Models;

class Program
{
    public static void Main(string[] args)
    {
        var storage = new MemoryStorageEngine();
        storage.LoadFromDisk();

        Console.WriteLine($"""
            ========================================
                     Atlas Database v0.1
            ========================================

            Loaded {storage.CountTables()} table(s).

            Type SQL statements and press Enter.

            Supported commands:
              CREATE TABLE
              INSERT INTO
              SELECT * FROM
              WHERE (=, >, <, >=, <=)

            Examples:
              CREATE TABLE Users (Id INT, Name STRING)
              INSERT INTO Users VALUES (1, 'Juan')
              SELECT * FROM Users
              SELECT * FROM Users WHERE Id >= 1

            Type EXIT to quit.

            ========================================
            """);

        while (true)
        {
            try
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (input?.Equals("EXIT", StringComparison.OrdinalIgnoreCase) == true)
                {
                    storage.Flush();
                    break;
                }

                var lexer = new Lexer(input);
                var tokens = lexer.Tokenize();

                var parser = new Parser(tokens);
                var statement = parser.Parse();

                var executor = new Executor(storage);
                var result = executor.Execute(statement);

                if (result is QueryResult query)
                {
                    PrintTable(query.Columns, query.Rows);
                    continue;
                }

                Console.WriteLine(result);
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex.Message}");
            }
        }
    }

    private static void PrintTable(List<Column> columns, List<Row> rows)
    {
        var widths = new int[columns.Count];

        for (int i = 0; i < columns.Count; i++)
        {
            widths[i] = columns[i].Name.Length;
        }

        foreach (var row in rows)
        {
            for (int i = 0; i < row.Values.Count; i++)
            {
                var value = row.Values[i].Value?.ToString() ?? "NULL";

                widths[i] = Math.Max(
                    widths[i],
                    value.Length);
            }
        }

        string Separator()
        {
            return "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
        }

        Console.WriteLine(Separator());

        Console.WriteLine(
            "| "
            + string.Join(" | ",
                columns.Select((c, i) =>
                    c.Name.PadRight(widths[i])))
            + " |");

        Console.WriteLine(Separator());

        foreach (var row in rows)
        {
            Console.WriteLine(
                "| "
                + string.Join(" | ",
                    row.Values.Select((v, i) =>
                        (v.Value?.ToString() ?? "NULL")
                            .PadRight(widths[i])))
                + " |");
        }

        Console.WriteLine(Separator());
        Console.WriteLine($"{rows.Count} row(s)");
    }
}