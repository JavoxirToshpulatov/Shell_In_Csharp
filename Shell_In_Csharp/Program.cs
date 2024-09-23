using Npgsql;
using Shell_In_Csharp;

internal class Program
{
    private static void Main()
    {
        Console.Write("Enter Host: ");
        string? host = Console.ReadLine();
        Console.Write("Enter port: ");
        string? port = Console.ReadLine();
        Console.Write("Enter Database: ");
        string? database = Console.ReadLine();
        Console.Write("Enter Usename: ");
        string? username = Console.ReadLine();
        Console.Write("Enter password: ");
        string? password = Console.ReadLine();
        var connection = SchemaMenu.ConnectToDatabase(host, port, database, username, password);

        using (var conn = connection)
        {
            Console.WriteLine("Successfully connected to database");
            Console.ReadLine();
            ArrowIndex(new List<string> { "Schemas" }, " ");
            //int key = ArrowIndex(ShowSchemas(conn), "Schemas");

            schema:
            List<string> list1 = new List<string>()
            {
                "\t\t1.Functions",
                "\t\t2.Procedures",
                "\t\t3.Tables",
                "\t\t4.Views",
                "\t\t5.Sequences",
                "\t\t6.Back"

            };

            int index = ArrowIndex(list1, " ");

            switch (index)
            {
                case 0:
                    SchemaMenu.RetrievePostgresFunctions(conn);
                    Console.ReadLine();
                    goto schema;
                    break;
                case 1:
                    SchemaMenu.RetrievePostgresFunctions(conn);
                    Console.ReadLine();
                    goto schema;
                    break;
                case 2:
                    SchemaMenu.DatabaseTables(conn);
                    Console.ReadLine();
                    goto schema;
                    break;
                case 3:
                    SchemaMenu.RetrievePostgresViews(conn);
                    Console.ReadLine();
                    goto schema;
                    break;
                case 4:
                    break;
            }
        }
    }

    
    
    public static int ArrowIndex(List<string> list, string name)
    {
        int selectIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t\t\t" + name);

            for (int i = 0; i < list.Count; i++)
            {
                if (i == selectIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(list[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            if (consoleKeyInfo.Key == ConsoleKey.UpArrow) selectIndex = (selectIndex - 1 + list.Count) % list.Count;
            else if (consoleKeyInfo.Key == ConsoleKey.DownArrow) selectIndex = (selectIndex + 1) % list.Count;
            else if (consoleKeyInfo.Key == ConsoleKey.Enter) return selectIndex;
        }
    }
}