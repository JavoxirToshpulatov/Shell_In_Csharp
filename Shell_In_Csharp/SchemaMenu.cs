﻿using Npgsql;
using System.Data;

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
                    "\t\t6.Back"
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
                    Databases(connection1);
                    break;
            }
        }

        public static List<string> Databases (NpgsqlConnection con) 
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

        public static NpgsqlConnection ConnectToServer(string host, string port,  string Username, string password)
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

        public static void InsertQuery(string tableName, Dictionary<string, object> columnValues, NpgsqlConnection con)
        {
            var columns = string.Join(", ", columnValues.Keys);
            var parameters = string.Join(", ", columnValues.Keys.Select(k => "@" + k));

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

            using (var command = new NpgsqlCommand(query, con))
            {
                foreach (var kvp in columnValues)
                {
                    command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        public static List<string> GetTableColumns(string tableName, NpgsqlConnection con)
        {
            var columns = new List<string>();
            var item = con.GetSchema("Columns", new string[] { null, null, tableName });

            foreach (DataRow row in item.Rows)
            {
                columns.Add((string)row["COLUMN_NAME"]);
            }

            return columns;
        }

        public static void InsertData(NpgsqlConnection con, string tableName)
        {
            // Get column names for the specified table
            var columns = GetTableColumns(tableName, con);
            var columnValues = new Dictionary<string, object>();

            Console.WriteLine($"Insert data into table: {tableName}");

            foreach (var column in columns)
            {
                Console.Write($"{column}: ");
                string input = Console.ReadLine();

                // Determine the data type for the column
                var columnType = GetColumnType(column, tableName, con);

                object value = null;

                // Try to convert the input based on the column type
                try
                {
                    if (columnType == typeof(int))
                    {
                        value = int.Parse(input);
                    }
                    else if (columnType == typeof(decimal))
                    {
                        value = decimal.Parse(input);
                    }
                    else if (columnType == typeof(short)) // Handle smallint
                    {
                        value = short.Parse(input);
                    }
                    else
                    {
                        value = string.IsNullOrEmpty(input) ? null : input; // Allow null values for strings
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Invalid input for column '{column}'. Expected type: {columnType.Name}.");
                    return; // Exit if input is invalid
                }

                columnValues[column] = value;
            }

            InsertQuery(tableName, columnValues, con);
            Console.WriteLine("Data inserted successfully!");
        }

        public static Type GetColumnType(string columnName, string tableName, NpgsqlConnection con)
        {
            var item = con.GetSchema("Columns", new string[] { null, null, tableName });
            foreach (DataRow row in item.Rows)
            {
                if ((string)row["COLUMN_NAME"] == columnName)
                {
                    // Map PostgreSQL types to .NET types
                    string dataType = row["DATA_TYPE"].ToString().ToLower();
                    return dataType switch
                    {
                        "smallint" => typeof(short),
                        "integer" => typeof(int),
                        "decimal" => typeof(decimal),
                        "varchar" => typeof(string),
                        "text" => typeof(string),
                        "date" => typeof(DateTime),
                        // Add other mappings as needed
                        _ => typeof(string) // Default to string
                    };
                }
            }
            return typeof(string); // Default to string if not found
        }

        public static void SelectQuery(string tableName, NpgsqlConnection con, string whereClause = null)
        {
            string query = $"SELECT * FROM {tableName}";

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " WHERE " + whereClause;
            }

            using (var command = new NpgsqlCommand(query, con))
            {
                using (var reader = command.ExecuteReader())
                {
                    // Check if there are any rows
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No data found.");
                        return;
                    }

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i),-30}"); 
                    }
                    Console.WriteLine();

                    Console.WriteLine(new string('-', 30 * reader.FieldCount));

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i],-30}"); 
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void UpdateData(NpgsqlConnection con, string tableName)
        {
            Console.Write("Enter the WHERE clause (e.g., id = 1): ");
            string whereClause = Console.ReadLine();
            var currentData = GetCurrentData(tableName, con, whereClause);

            if (currentData == null || currentData.Count == 0)
            {
                Console.WriteLine("No data found for the specified condition.");
                return;
            }

            Console.WriteLine("Current Data:");
            foreach (var kvp in currentData)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            var columnValues = new Dictionary<string, object>();

            foreach (var kvp in currentData)
            {
                Console.Write($"Enter new value for {kvp.Key} (leave blank to keep current value): ");
                string input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    if (kvp.Key == "state_id") // Example for smallint
                    {
                        if (short.TryParse(input, out short smallIntValue))
                        {
                            columnValues[kvp.Key] = smallIntValue;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid input for {kvp.Key}. Please enter a valid smallint.");
                            return;
                        }
                    }
                    else
                    {
                        columnValues[kvp.Key] = input; // For other types, handle as string
                    }
                }
                else
                {
                    columnValues[kvp.Key] = kvp.Value; // Keep the current value
                }
            }

            // Call the UpdateQuery method
            UpdateQuery(tableName, columnValues, whereClause, con);
        }

        public static Dictionary<string, object> GetCurrentData(string tableName, NpgsqlConnection con, string whereClause)
        {
            string query = $"SELECT * FROM {tableName} WHERE {whereClause}";

            using (var command = new NpgsqlCommand(query, con))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read()) return null;

                var data = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    data[reader.GetName(i)] = reader[i];
                }
                return data;
            }
        }

        public static void UpdateQuery(string tableName, Dictionary<string, object> columnValues, string whereClause, NpgsqlConnection con)
        {
            if (columnValues == null || columnValues.Count == 0)
            {
                Console.WriteLine("No columns to update.");
                return;
            }

            // Construct the SET clause
            var setClauses = new List<string>();
            foreach (var column in columnValues.Keys)
            {
                setClauses.Add($"{column} = @{column}");
            }

            string setClause = string.Join(", ", setClauses);
            string query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

            using (var command = new NpgsqlCommand(query, con))
            {
                // Add parameters to the command
                foreach (var kvp in columnValues)
                {
                    command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                }

                // Execute the update command
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) updated.");
            }
        }

        public static void DeleteQuery(string tableName, string whereClause, NpgsqlConnection con)
        {
            // Ensure the WHERE clause is not empty
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                Console.WriteLine("WHERE clause cannot be empty.");
                return;
            }

            // Construct the DELETE query
            string query = $"DELETE FROM {tableName} WHERE {whereClause}";

            using (var command = new NpgsqlCommand(query, con))
            {
                // Execute the delete command
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) deleted.");
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
