using Shell_In_Csharp;

internal class Program
{
    private static void Main()
    {
        try
        {
            bool exit = false;

            server:
            Console.Write("Enter Host[localhost]: ");
            string? host = Console.ReadLine();

            if (string.IsNullOrEmpty(host))
                host = "localhost";

            port:
            Console.Write("Enter port[5432]: ");
            string? port = Console.ReadLine();

            if (string.IsNullOrEmpty(port))
                port = "5432";
            int result=0;
            if (!int.TryParse(port, out result))
            {
                Console.WriteLine("Port cannot be like a string");
                goto port;
            }

            Console.Write("Enter Username[postgres]: ");
            string? username = Console.ReadLine();

            if (string.IsNullOrEmpty(username))
                username = "postgres";

            password:
            Console.Write("Enter password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty");
                goto password;
            }

            while (!exit)
            {
                try
                {
                    var connection = SchemaMenu.ConnectToServer(host, port, username, password);

                    using (var conn = connection)
                    {
                        Console.WriteLine("Successfully connected to server");
                        Console.ReadLine();

                        List<string> databases = SchemaMenu.Databases(conn);
                        databaseGoTo:
                        int databseIndex = ArrowIndex(databases, "Databases");

                        if (databases[databseIndex]== "Back to Host")
                        {
                            Console.Clear();
                            goto server;
                        }

                        var connection1 = SchemaMenu.ConnectToDatabase(host, port, username, password, databases[databseIndex]);

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
                                List<string> listFunctions = SchemaMenu.RetrievePostgresFunctions(connection1);
                                if (listFunctions.Count == 0)
                                {
                                    Console.WriteLine("No functions yet");
                                    Console.ReadLine();
                                    goto schema;
                                }
                                int function = ArrowIndex(listFunctions, " ");
                                Console.ReadLine();
                                goto schema;
                            case 1:
                                SchemaMenu.RetrievePostgresProcedures(connection1);
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
                                        SchemaMenu.CreateTable(connection1);
                                        Console.ReadLine();
                                        goto schema;
                                    case 1:
                                        List<string> tables = Tables.DatabaseTables(connection1);
                                        if (tables.Count == 0)
                                        {
                                            Console.WriteLine("No Tables yet");
                                            Console.ReadLine();
                                            goto schema;
                                        }
                                        int keyTables = ArrowIndex(tables, " ");
                                        string selectedTable = tables[keyTables];
                                    tables:
                                        int key = ArrowIndex(tablesProperty, " ");

                                        switch (key)
                                        {
                                            case 0:
                                                List<string> columns = SchemaMenu.GetTableColumns(selectedTable, connection1);
                                                ArrowIndex(columns, "Columns");
                                                Console.ReadLine();
                                                goto tables;
                                            case 1:
                                                SchemaMenu.InsertData(connection1, selectedTable);
                                                Console.ReadLine();
                                                goto tables;
                                            case 2:
                                                //select
                                                SchemaMenu.SelectQuery(selectedTable, connection1);
                                                Console.ReadLine();
                                                goto tables;
                                            case 3:
                                                SchemaMenu.UpdateData(connection1, selectedTable);
                                                Console.ReadLine();
                                                goto tables;
                                            //update
                                            case 4:
                                                Console.Write("Enter the WHERE clause for deletion (e.g., id = 1): ");
                                                string whereClause = Console.ReadLine();
                                                SchemaMenu.DeleteQuery(selectedTable, whereClause, connection1);
                                                Console.ReadLine();
                                                goto tables;
                                            //delete
                                            case 5:
                                                SchemaMenu.AddColumn(selectedTable, connection1);
                                                Console.ReadLine();
                                                goto tables;
                                            case 6:
                                                SchemaMenu.ModifyColumn(selectedTable, connection1);
                                                Console.ReadLine();
                                                goto tables;
                                            case 7:
                                                SchemaMenu.DropColumn(selectedTable, connection1);
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
                                SchemaMenu.RetrievePostgresViews(connection1);
                                Console.ReadLine();
                                goto schema;
                            case 4:
                                SchemaMenu.RetrievePostgresSequences(connection1);
                                Console.ReadLine();
                                goto schema;
                            case 5:
                                goto databaseGoTo;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
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