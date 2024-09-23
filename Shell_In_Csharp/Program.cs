using Shell_In_Csharp;
using System;

internal class Program
{
    private static void Main()
    {
        try
        {
            
            bool exit = false;
            while (!exit)
            {

                Console.Write("Enter Host: ");
                string? host = Console.ReadLine();
                if (string.IsNullOrEmpty(host))
                {
                    host = "localhost";
                }
                Console.Write("Enter port: ");
                string? port = Console.ReadLine();
                if (string.IsNullOrEmpty(port))
                {
                    port = "5432";
                }
                Console.Write("Enter Database: ");
                string? database = Console.ReadLine();
                if (string.IsNullOrEmpty(database))
                {
                    database = "postgres";
                }
                Console.Write("Enter Username: ");
                string? username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                {
                    username = "postgres";
                }
                Console.Write("Enter password: ");
                string? password = Console.ReadLine();
                var connection = SchemaMenu.ConnectToDatabase(host, port, database, username, password);

                using (var conn = connection)
                {
                    conn.Open();
                    Console.WriteLine("Successfully connected to database");
                    Console.ReadLine();
                    ArrowIndex(new List<string> { "Schemas" }, " ");

                schema:
                    List<string> list1 = new List<string>()
            {
                "\t\t1.Functions",
                "\t\t2.Procedures",
                "\t\t3.Tables",
                "\t\t4.Views",
                "\t\t5.Sequences",
                "\t\t6.Enter query",
                "\t\t7.Back"

            };

                    List<string> tablesProperty = new List<string>()
            {
                "Columns",
                "Insert data",
                "Select data",
                "Update data",
                "Delete data",
                "Add Column",
                "Modify Column",
                "Drop Column",
                "Back"

            };

                    int index = ArrowIndex(list1, " ");

                    switch (index)
                    {
                        case 0:
                            List<string> listFunctions = SchemaMenu.RetrievePostgresFunctions(conn);
                            int function = ArrowIndex(listFunctions, " ");
                            Console.ReadLine();
                            goto schema;
                        case 1:
                            SchemaMenu.RetrievePostgresProcedures(conn);
                            Console.ReadLine();
                            goto schema;
                        case 2:
                            List<string> TableStrings = new List<string>()
                    {
                        "Create table",
                        "Select table",
                        "Back"
                    };
                            int IndexTable = ArrowIndex(TableStrings, " ");
                            switch (IndexTable)
                            {
                                case 0:
                                    SchemaMenu.CreateTable(conn);
                                    Console.ReadLine();
                                    goto schema;
                                case 1:
                                    List<string> tables = SchemaMenu.DatabaseTables(conn);
                                    int keyTables = ArrowIndex(tables, " ");
                                    string selectedTable = tables[keyTables];
                                tables:
                                    int key = ArrowIndex(tablesProperty, " ");

                                    switch (key)
                                    {
                                        case 0:
                                            List<string> columns = SchemaMenu.GetTableColumns(selectedTable, conn);
                                            ArrowIndex(columns, "Columns");
                                            Console.ReadLine();
                                            goto tables;
                                        case 1:
                                            SchemaMenu.InsertData(conn, selectedTable);
                                            Console.ReadLine();
                                            goto tables;
                                            break;
                                        case 2:
                                            //select
                                            SchemaMenu.SelectQuery(selectedTable, conn);
                                            Console.ReadLine();
                                            goto tables;
                                        case 3:
                                            SchemaMenu.UpdateData(conn, selectedTable);
                                            Console.ReadLine();
                                            goto tables;
                                            //update
                                        case 4:
                                            Console.Write("Enter the WHERE clause for deletion (e.g., id = 1): ");
                                            string whereClause = Console.ReadLine();
                                            SchemaMenu.DeleteQuery(selectedTable, whereClause, conn);
                                            Console.ReadLine();
                                            goto tables;
                                            //delete
                                        case 5:
                                            SchemaMenu.AddColumn(selectedTable, conn);
                                            Console.ReadLine();
                                            goto tables;
                                        case 6:
                                            SchemaMenu.ModifyColumn(selectedTable, conn);
                                            Console.ReadLine();
                                            goto tables;
                                        case 7:
                                            SchemaMenu.DropColumn(selectedTable, conn);
                                            Console.ReadLine();
                                            goto tables;
                                        case 8:
                                            goto schema;
                                    }
                                    Console.ReadLine();
                                    break;
                                case 2:
                                    goto schema;

                            }
                            break;
                        case 3:
                            SchemaMenu.RetrievePostgresViews(conn);
                            Console.ReadLine();
                            goto schema;
                        case 4:
                            SchemaMenu.RetrievePostgresSequences(conn);
                            Console.ReadLine();
                            goto schema;
                        case 5:
                            Console.WriteLine("Enter query: ");
                            string query = Console.ReadLine();
                            SchemaMenu.QueryTool(query, conn);
                            Console.ReadLine();
                            break;
                        case 6:
                            exit = true;
                            break;
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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