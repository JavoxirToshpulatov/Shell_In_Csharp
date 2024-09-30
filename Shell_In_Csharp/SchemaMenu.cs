using Npgsql;

namespace Shell_In_Csharp
{
    public static class SchemaMenu
    {
        public static void MainFeatures(NpgsqlConnection connection1)
        {
            List<string> list1 = new List<string>()
                {
                    "\t\t1.Functions",
                    "\t\t2.Procedures",
                    "\t\t3.Tables",
                    "\t\t4.Views",
                    "\t\t5.Sequences",
                    "\t\t6.Query Tool",
                    "\t\t7.Back"
                };
        Features:
            int index = Program.ArrowIndex(list1, " ");

            switch (index)
            {
                case 0:
                    List<string> listFunctions = SchemaMenu.RetrievePostgresFunctions(connection1);
                    if (listFunctions.Count == 0)
                    {
                        Console.WriteLine("No functions yet");
                        Console.ReadLine();
                        goto Features;
                    }
                    int function = Program.ArrowIndex(listFunctions, " ");
                    goto Features;
                case 1:
                    SchemaMenu.RetrievePostgresProcedures(connection1);
                    Console.ReadLine();
                    goto Features;
                case 2:
                    Tables.TablesMenu(connection1);
                    break;
                case 3:
                    SchemaMenu.RetrievePostgresViews(connection1);
                    Console.ReadLine();
                    goto Features;
                case 4:
                    SchemaMenu.RetrievePostgresSequences(connection1);
                    Console.ReadLine();
                    goto Features;
                case 5:
                    Console.Write("Enter query: ");
                    string query = Console.ReadLine();
                    SchemaMenu.QueryTool(connection1, query);
                    break;
                case 6:
                    Databases(connection1);
                    break;
            }
        }

        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    //Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }

        public static void QueryTool(NpgsqlConnection con, string query)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }

        }

        public static List<string> Databases(NpgsqlConnection con)
        {
            List<string> result = new List<string>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false;", con);
            var databases = cmd.ExecuteReader();

            while (databases.Read())
                result.Add(databases.GetString(0));
            result.Add("Back to Host");
            return result;
            //demo
        }

        public static NpgsqlConnection ConnectToServer(string host, string port, string Username, string password)
        {
            string connectionString = $"Host={host}; port={port}; User Id={Username}; Password={password}";
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            return con;
        }

        public static NpgsqlConnection ConnectToDatabase(string host, string port, string Username, string password, string database)
        {
            string connectionString = $"Host={host}; port={port}; Database={database}; User Id={Username}; Password={password}";

            NpgsqlConnection newConnection = new NpgsqlConnection(connectionString);
            newConnection.Open();
            return newConnection;
        }

        public static List<string> RetrievePostgresFunctions(NpgsqlConnection con)
        {
            string query = @"
                SELECT 
                    p.proname AS function_name
                    FROM pg_catalog.pg_proc p
                    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = p.pronamespace
                    WHERE n.nspname = 'public'
                    ORDER BY function_name;
                ";
            List<string> listFunctions = new List<string>();
            using (var cmd = new NpgsqlCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listFunctions.Add($"Function: {reader["function_name"]}");
                    }
                }
            }
            return listFunctions;
        }

        public static void RetrievePostgresProcedures(NpgsqlConnection con)
        {

            string query = @"
                SELECT 
                    p.proname AS procedure_name
                    FROM pg_catalog.pg_proc p
                    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = p.pronamespace
                    WHERE n.nspname = 'public'
                    AND p.prokind = 'p'  -- 'p' indicates stored procedures
                    ORDER BY procedure_name;
                ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    int t = 0;
                    while (reader.Read())
                    {
                        Console.WriteLine($"Procedure: {reader["procedure_name"]}");
                        t++;
                    }
                    if (t == 0)
                        Console.WriteLine("There are not procedures yet");
                }
            }
        }

        public static void RetrievePostgresViews(NpgsqlConnection con)
        {
            string query = @"
                SELECT 
                    table_name AS view_name
                    FROM information_schema.views
                    WHERE table_schema = 'public'
                    ORDER BY view_name;
                ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                //con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    int t = 0;
                    while (reader.Read())
                    {
                        Console.WriteLine($"View: {reader["view_name"]}");
                        t++;
                    }
                    if (t == 0) Console.WriteLine("No views yet");
                }
                //con.Close();
            }
        }

        public static void RetrievePostgresSequences(NpgsqlConnection con)
        {
            try
            {
                string query = "SELECT DISTINCT sequencename AS SequenceName FROM pg_sequences";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int t = 0;
                        while (reader.Read())
                        {
                            Console.WriteLine($"Sequence Name: {reader["SequenceName"]}");
                            t++;
                        }
                        if (t == 0) Console.WriteLine("No sequences yet");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void CreateTable(NpgsqlConnection con)
        {
            Console.Write("Enter the name of the table to create: ");
            string tableName = Console.ReadLine();

            Console.WriteLine("Enter the columns in the format: column_name data_type, e.g., id SERIAL PRIMARY KEY, name VARCHAR(100), age SMALLINT");
            string columns = Console.ReadLine();

            // Construct the CREATE TABLE query
            string query = $"CREATE TABLE {tableName} ({columns});";

            using (var command = new NpgsqlCommand(query, con))
            {
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Table '{tableName}' created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating table: {ex.Message}");
                }
            }
        }

        public static void AddColumn(string tableName, NpgsqlConnection con)
        {
            Console.Write("Enter the column to add (e.g., new_column_name data_type): ");
            string addColumn = Console.ReadLine();
            string query = $"ALTER TABLE {tableName} ADD COLUMN {addColumn};";
            using (var command = new NpgsqlCommand(query, con))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Successful");
            }
        }

        public static void ModifyColumn(string tableName, NpgsqlConnection con)
        {
            try
            {
                Console.Write("Enter the column to modify (e.g., existing_column_name new_data_type): ");
                string modifyColumn = Console.ReadLine();
                string query = $"ALTER TABLE {tableName} ALTER COLUMN {modifyColumn};";
                using (var command = new NpgsqlCommand(query, con))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Successful");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DropColumn(string tableName, NpgsqlConnection con)
        {
            Console.Write("Enter the column to drop: ");
            string dropColumn = Console.ReadLine();
            string query = $"ALTER TABLE {tableName} DROP COLUMN {dropColumn};";
            using (var command = new NpgsqlCommand(query, con))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
